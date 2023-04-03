﻿using Basket.API.Service.Interfaces;
using Shared.Configurations;

namespace Basket.API.Service
{
    public class BasketEmailTemplateService : EmailTemplateService, IEmailTemplateService
    {
        public BasketEmailTemplateService(BackgroundJobSettings settings) : base(settings)
        {
        }

        public string GenerateReminderCheckoutOrderEmail(string username, string checkoutUrl = "basket/checkout")
        {
            var _checkoutUrl = $"{BackgroundJobSettings.ApiGwUrl}/{checkoutUrl}/{username}";
            var emailText = ReadEmailTemplateContent("reminder-checkout-order");
            var emailReplacedText = emailText.Replace("[username]", username)
                .Replace("[checkoutUrl]", checkoutUrl);

            return emailReplacedText;
        }
    }
}
