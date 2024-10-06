using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public interface IEmailSender
    {
        void SendEmail(Message message, List<StockResult> results  , string clientName);
        Task SendEmailAsync(Message message, List<StockResult> results , string clientName);

        Task SendEmailAsyncToCustomerWithBookingDetails(Message message, Guid id);

        Task SendEmailAsyncToCustomer(Message message);

        Task SendEmailAsyncToCustomerNotConfirmedBooking(Message message, string Comment, string CustomerEmail);
    }
}
