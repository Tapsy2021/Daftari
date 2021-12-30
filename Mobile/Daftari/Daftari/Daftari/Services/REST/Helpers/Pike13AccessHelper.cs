using Daftari.Models;
using Daftari.Services.REST.ViewModels;
using Daftari.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Daftari.Services.REST.Helpers
{
    public class Pike13AccessHelper : BaseHelper
    {
        public static async Task<List<Visit>> GetVisitsAsync(object param, CancellationTokenSource ct_Token)
        {
            var queryString = GetQueryString(param);
            try
            {
                return await new RestService().GetListAsync<Visit>((Application.Current as App), Constants.URLs.Get_Visits + queryString, ct_Token.Token);
            }
            catch { }
            return null;
        }

        public static async Task<SimpleResult> CancelVisit(object param, CancellationTokenSource ct_Token)
        {
            var tcs = new TaskCompletionSource<SimpleResult>();

            try
            {
                var deroResult = await new RestService().PostAsync((Application.Current as App), Constants.URLs.Cancel_Visit, param, ct_Token.Token);

                if (deroResult != null)
                {
                    if (deroResult.IsSuccess)
                    {
                        tcs.SetResult(new SimpleResult { IsSuccess = true, Message = deroResult.Body.ToString() });
                    }
                    else
                    {
                        if (deroResult.ErrorMessages != null && deroResult.ErrorMessages.Any())
                        {
                            throw new ValidationException(deroResult.ErrorMessages[0]);
                        }
                        throw new ValidationException(deroResult.Body.ToString());
                    }
                }
                else
                {
                    throw new ValidationException("Failed to connect!");
                }
            }
            catch (ValidationException ex)
            {
                tcs.SetResult(new SimpleResult { IsSuccess = false, Message = ex.Message });
            }
            catch (OperationCanceledException)
            {
                tcs.SetResult(new SimpleResult { IsSuccess = false });
                // do nothing
            }
            catch (HttpException ex)
            {
                tcs.SetResult(new SimpleResult { IsSuccess = false, Message = ex.Message });
            }
            catch (Exception)
            {
                tcs.SetResult(new SimpleResult { IsSuccess = false, Message = Constants.Messages.Network_Error });
            }
            finally
            {
                //tcs.TrySetResult(new SimpleResult { IsSuccess = false, Message = Constants.Messages.Network_Error });
            }

            return await tcs.Task;
        }
    }
}
