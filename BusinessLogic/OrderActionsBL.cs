
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Net;
using System.Text;
using LiqPay.SDK;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Main.Actions;
using sushi_backend.BusinessLogic;
using sushi_backend.Context;
using sushi_backend.Interfaces;
using sushi_backend.Models;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Main.DBContext;
using WebShop.Main.Hubs;
using WebShop.Main.Interfaces;
using WebShop.Models;

namespace WebShop.Main.BusinessLogic
{
    public class OrderActionsBL : IOrderActionsBL
    {
        private ShopContext _context;

        private IEmailSender _emailSenderBL;

        private readonly IHubContext<OrderHub> _hubContext;

        public OrderActionsBL(ShopContext context, IHubContext<OrderHub> hubContext, IEmailSender emailSenderBL)
        {
            _context = context;
            _hubContext = hubContext;
            _emailSenderBL = emailSenderBL;
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await _context.users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<OrderResponsModel> CreateNewOrder(List<CartItemModel> cartItems, DeliveryOptionsModel deliveryOptions, PaymentMethod paymentMethod, ContactInfo contactInfo, PromoCodeOrderModel promocode)
        {
            var orderId = Guid.NewGuid();

            double totalPrice = GetTotalPrice(cartItems);

            double discount =0;

            if (deliveryOptions.DeliveryType == DeliveryType.PicUp)
            {
                discount += totalPrice / 10;
                totalPrice = totalPrice - totalPrice / 10;
            }

            if (promocode.PromoUsed)
            {
                var code = await _context.promocodes.FirstOrDefaultAsync(x => x.Code == promocode.UsedPromoCode);

                discount += code.Discount;

                totalPrice -= code.Discount;
            }

            var orderProduct = "";

            TimeZoneInfo ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");

            DateTime utcTime = DateTime.UtcNow;

            DateTime newDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ukraineTimeZone);

            var deliveryOptionsNew = new DeliveryOptions
            {
                DeliveryOptionsId = Guid.NewGuid(),
                Address = deliveryOptions.Address,
                Latitude = deliveryOptions.Latitude,
                Longitude = deliveryOptions.Longitude,
                DeliveryTimeOptions = deliveryOptions.DeliveryTimeOptions,
                DeliveryTime = deliveryOptions.DeliveryTime,
                DeliveryType = deliveryOptions.DeliveryType,
                OrderId = orderId,
            };

            var newOrder = new Order()
            {
                OrderId = orderId,
                TotalPrice = totalPrice,
                OrderTime = newDate,
                Name = contactInfo.Name,
                SurName = contactInfo.SurName,
                PhoneNumber = contactInfo.PhoneNumber,
                Email = contactInfo.Email,
                PromoUsed = promocode.PromoUsed,
                OrderNumber = _context.orders.OrderBy(x => x.OrderNumber).Last().OrderNumber + 1,
                UsedPromoCode = promocode.UsedPromoCode,
                OrderStatus = OrderStatus.AwaitingPayment,
                PaymentMethod = paymentMethod,
                Discount = discount,
                DeliveryOptions = deliveryOptionsNew,
                OrderMessage = "",
            };

            var products = _context.products;

            foreach (var item in cartItems)
            {
                var product = products.FirstOrDefault(x => x.ProductId == item.ProductId);

                _context.orderLists.Add(new OrderList
                {
                    Product = product,
                    Order = newOrder,
                    Count = item.Count,
                    Name = product.Name,
                    Img = product.Image,
                    Price = product.Price
                });
                orderProduct += $" {product.Name} x {item.Count} \n";
            }

            _context.orders.Add(newOrder);

            if (paymentMethod == PaymentMethod.CardOnline)
            {
                await _context.SaveChangesAsync();

                var respon = await GoToPayment(contactInfo.Email, totalPrice, orderId);

                return respon;
            }
            else
            {
                newOrder.OrderStatus = OrderStatus.AwaitingConfirm;
            }

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("MakeOrder", newOrder);

            SentNotofication(newOrder, orderProduct);

            var orderRespons = new OrderResponsModel
            {
                Href = $"https://umamigroup.click/order-info/{newOrder.OrderId.ToString()}",
                OrderId = orderId
            };

            return orderRespons;
        }


        public string SentNotofication(Order newOrder, string orderProduct)
        {
            var telegramInfo = $"Нове замовлення №{newOrder.OrderNumber}! \n Час замовлення:{newOrder.OrderTime} \n  Продукти: \n" + orderProduct;

            //_emailSenderBL.SentEmail(telegramInfo);

            string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
            string apiToken = "6039771294:AAGdh-7JAWGtRnY9FhTi5S4mKxxUXVB3dIw";
            string chatId = "481292931";
            string text = $"{telegramInfo}";
            urlString = String.Format(urlString, apiToken, chatId, text);
            WebRequest request = WebRequest.Create(urlString);
            Stream rs = request.GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(rs);
            string line = "";
            StringBuilder sb = new StringBuilder();
            while (line != null)
            {
                line = reader.ReadLine();
                if (line != null)
                    sb.Append(line);
            }
            return sb.ToString();
        }

        public async Task<Order> GetOrder(Guid orderId) =>
            await _context.orders.Where(x => x.OrderId == orderId).Include(x => x.DeliveryOptions).FirstOrDefaultAsync();
        
        public bool CheckCountOfProducts(List<CartItemModel> cartItems)
        {
            var products = _context.products;

            foreach (var item in cartItems)
            {
                var product = products.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (product.Available < item.Count && product.Available != 1)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<Order> GetOrderById(string orderId) =>
            await _context.orders
            .Where(x => x.OrderId == Guid.Parse(orderId))
            .Include(x => x.OrderLists)
            .Include(x=> x.DeliveryOptions)
            .FirstOrDefaultAsync();

        public async Task<List<Order>> GetUserOrders(string[] orders) =>
            await _context.orders
            .Where(x => orders.Any(xo=> xo == x.OrderId.ToString()))
            .OrderByDescending(x => x.OrderNumber)
            .Include(x => x.OrderLists)
            .Include(x => x.DeliveryOptions)
            .ToListAsync();

        public async Task<string> ChangeOrderStatus(Order order, OrderStatus orderStatus)
        {
            order.OrderStatus = orderStatus;

            var orderDTO = _context.orders
                .Where(x => x.OrderId == order.OrderId)
                .Include(x => x.OrderLists)
                .Include(x => x.DeliveryOptions)
                .FirstOrDefault();

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("MakeOrder", orderDTO);

            return "Ok";
        }

        public async Task<List<Order>> GetNewOrders()
        {
            return await _context.orders.Where(x => x.OrderStatus != OrderStatus.Completed && x.OrderStatus != OrderStatus.Declined && x.OrderStatus != OrderStatus.Canceled && x.OrderStatus != OrderStatus.AwaitingPayment)
                .Include(x => x.OrderLists)
                .Include(x=> x.DeliveryOptions)
                .OrderByDescending(x => x.OrderTime)
                .ToListAsync();
        }

        public int GetTotalPrice(List<CartItemModel> cart)
        {
            if (cart != null)
            {
                int totalPrice = 0;

                var products = _context.products;

                foreach (var item in cart)
                {
                    totalPrice += products.FirstOrDefault(x => x.ProductId == item.ProductId).Price * item.Count;
                }

                return totalPrice;
            }
            return 0;
        }

        public static decimal CalculateDiscountedPrice(List<Product> products)
        {
            decimal totalPrice = products.Sum(p => p.Price); // Обчислюємо загальну ціну замовлення
            int pizzaCount = products.Count(p => p.Category.CategoryName == "Сети"); // Обчислюємо кількість піц в замовленні
            if (pizzaCount >= 3) // Якщо в замовленні є достатня кількість піц для знижки
            {
                decimal cheapestPizzaPrice = products.Where(p => p.Category.CategoryName == "Pizza").Min(p => p.Price); // Обчислюємо ціну найдешевшої піци
                decimal discount = (pizzaCount / 3) * cheapestPizzaPrice; // Знижка дорівнює ціні найдешевшої піци, помноженій на кількість разів, які можна застосувати знижку
                return totalPrice - discount; // Повертаємо нову ціну замовлення з урахуванням знижки
            }
            else // Якщо в замовленні менше піц, ніж потрібно для знижки
            {
                return totalPrice; // Повертаємо загальну ціну замовлення без знижки
            }
        }

        public async Task<OrderResponsModel> GoToPayment(string email, double amount, Guid orderId)
        {
            var invoiceRequest = new LiqPayRequest
            {
                Email = "example@gmail.com",
                Amount = amount,
                Currency = "UAH",
                OrderId = orderId.ToString(),
                Action = LiqPayRequestAction.InvoiceSend,
                ResultUrl = $"https://umamigroup.click/order-info/{orderId.ToString()}",
                Language = LiqPayRequestLanguage.UK,
                ServerUrl = "https://web-shop.herokuapp.com/OrderActions/PaymentStatus",
            };

            var liqPayClient = new LiqPayClient("sandbox_i35438868943", "sandbox_hk7Vbmn1Li9UOa3P13ZYyOZSnac8JlzWa96IJYZz");

            var response = await liqPayClient.RequestAsync("request", invoiceRequest);

            var orderRespons = new OrderResponsModel { Href = response.Href, OrderId = orderId };
            return orderRespons;
        }

        public bool ConfirmPayment(string data, string signature)
        {
            string decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(data));

            string expectedSignature = Convert.ToBase64String(ComputeHash("sandbox_hk7Vbmn1Li9UOa3P13ZYyOZSnac8JlzWa96IJYZz" + data + "sandbox_hk7Vbmn1Li9UOa3P13ZYyOZSnac8JlzWa96IJYZz"));
            if (signature != expectedSignature)
            {
                return false;
            }

            dynamic result = JsonConvert.DeserializeObject(decodedData);

            string orderId = result.order_id;

            string status = result.status;

            if(status != "success")
            {
                return false;
            }

            var products = _context.products;

            var order = _context.orders.Where(x => x.OrderId.ToString() == orderId).Include(x=> x.OrderLists).Include(x=> x.DeliveryOptions).FirstOrDefault();

            string orderProduct = "";

            foreach (var item in order.OrderLists)
            {
                var product = products.FirstOrDefault(x => x.ProductId == item.ProductId);
               
                orderProduct += $" {product.Name} x {item.Count} \n";
            }

            SentNotofication(order, orderProduct);

            order.OrderStatus = OrderStatus.AwaitingConfirm;

            _context.SaveChanges();

            _hubContext.Clients.All.SendAsync("MakeOrder", order);


            return true;

        }

        private byte[] ComputeHash(string input)
        {
            using (var sha1 = new System.Security.Cryptography.SHA1Managed())
            {
                return sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }
    }
}

