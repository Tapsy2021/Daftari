using Daftari.Services.REST.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Daftari.Services.REST.Helpers
{
    public class AuthHelper
    {
        //public static async Task<SimpleResult> Register(UserViewModel user, CancellationTokenSource ct_Token)
        //{
        //    var tcs = new TaskCompletionSource<SimpleResult>();

        //    try
        //    {
        //        var deroResult = await new RestService().PostAsync((Application.Current as App), Constants.URLs.Sign_Up, user, ct_Token.Token);

        //        if (deroResult != null)
        //        {
        //            if (deroResult.IsSuccess)
        //            {
        //                tcs.SetResult(new SimpleResult { IsSuccess = true, Message = deroResult.Body.ToString() });
        //            }
        //            else
        //            {
        //                if (deroResult.ErrorMessages != null && deroResult.ErrorMessages.Any())
        //                {
        //                    throw new ValidationException(deroResult.ErrorMessages[0]);
        //                }
        //                throw new ValidationException(deroResult.Body.ToString());
        //            }
        //        }
        //        else
        //        {
        //            throw new ValidationException("Failed to connect!");
        //        }
        //    }
        //    catch (ValidationException ex)
        //    {
        //        tcs.SetResult(new SimpleResult { IsSuccess = false, Message = ex.Message });
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        tcs.SetResult(new SimpleResult { IsSuccess = false });
        //        // do nothing
        //    }
        //    catch (HttpException ex)
        //    {
        //        tcs.SetResult(new SimpleResult { IsSuccess = false, Message = ex.Message });
        //    }
        //    catch (Exception)
        //    {
        //        tcs.SetResult(new SimpleResult { IsSuccess = false, Message = Constants.Messages.Network_Error });
        //    }
        //    finally
        //    {
        //        //tcs.TrySetResult(new SimpleResult { IsSuccess = false, Message = Constants.Messages.Network_Error });
        //    }

        //    return await tcs.Task;
        //}
    }
}
