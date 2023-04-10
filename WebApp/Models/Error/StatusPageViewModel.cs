using Common.Constants;
using Common.DataTransferObjects.ErrorLog;

namespace WebApp.Models.Error
{
    public class StatusPageViewModel
    {
        public int StatusCode { get; private set; }
        public string Title { get; private set; }
        public string SubTitle { get; private set; }
        public string Message { get; set; }
        public string ImagePath { get; private set; }
        public string TraceId { get; private set; }

        public StatusPageViewModel()
        {

        }

        public StatusPageViewModel(ErrorMessage errorMessage)
        {
            TraceId = errorMessage.TraceId;

            switch (errorMessage.Type)
            {
                case ErrorMessageTypeConstant.NotFound:
                    Title = StatusCodeConstant.Title404;
                    SubTitle = StatusCodeConstant.SubTitle404;
                    Message = StatusCodeConstant.Message404;
                    ImagePath = StatusCodeConstant.ImagePath404;
                    break;
                case ErrorMessageTypeConstant.BadRequest:
                case ErrorMessageTypeConstant.ArgumentException:
                case ErrorMessageTypeConstant.InternalServerException:
                    Title = StatusCodeConstant.Title500;
                    SubTitle = string.IsNullOrEmpty(TraceId) ? null : string.Format(StatusCodeConstant.SubTitle500, TraceId);
                    Message = errorMessage.Message;
                    ImagePath = StatusCodeConstant.ImagePath500;
                    break;
                default:
                    Title = StatusCodeConstant.TitleUnknown;
                    SubTitle = StatusCodeConstant.SubTitleUnknown;
                    Message = StatusCodeConstant.MessageUnknown;
                    ImagePath = StatusCodeConstant.ImagePathUnknown;
                    break;
            }
        }

        public StatusPageViewModel(int statusCode, string user)
        {
            StatusCode = statusCode;

            switch (StatusCode)
            {
                case 403:
                case 401:
                    Title = StatusCodeConstant.Title401;
                    SubTitle = string.Format(StatusCodeConstant.SubTitle401, user);
                    Message = StatusCodeConstant.Message401;
                    ImagePath = StatusCodeConstant.ImagePath401;
                    break;
                case 404:
                    Title = StatusCodeConstant.Title404;
                    SubTitle = StatusCodeConstant.SubTitle404;
                    Message = StatusCodeConstant.Message404;
                    ImagePath = StatusCodeConstant.ImagePath404;
                    break;
                default:
                    Title = StatusCodeConstant.TitleUnknown;
                    SubTitle = StatusCodeConstant.SubTitleUnknown;
                    Message = StatusCodeConstant.MessageUnknown;
                    ImagePath = StatusCodeConstant.ImagePathUnknown;
                    break;
            }
        }
    }
}
