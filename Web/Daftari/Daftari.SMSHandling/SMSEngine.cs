using Daftari.SMSHandling.DAL;
using Daftari.SMSHandling.Enums;
using Daftari.SMSHandling.Models;
using Daftari.SMSHandling.net.ismartsms;
using Daftari.SMSHandling.TamimahSMS;
using LukeApps.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;

namespace Daftari.SMSHandling
{
    public class SMSEngine : IDisposable
    {
        public SMSEngine(string Username, bool? OverrideEnableSMS = null)
        {
            _isSMSEnabled = OverrideEnableSMS ?? Boolean.Parse(ConfigurationManager.AppSettings["EnableSMS"]);
            _currentUsername = Username;
        }

        private SMSContext db = new SMSContext();
        private SMSConfig _smsconfig;
        private string _currentUsername;
        private SMSLog _smslog;
        private bool _isSMSEnabled;

        public async Task<int> SendAsync(SMSMessage sms)
        {
            if (_isSMSEnabled)
            {
                sendSMS(sms);
                logUserinContext(_currentUsername);
                db.SMSLogs.Add(_smslog);
                return await db.SaveChangesAsync();
            }
            return 0;
        }

        private void sendSMS(SMSMessage sms)
        {
            setSMSConfig(sms.SMSAPI);

            switch (sms.SMSAPI)
            {
                case SMSAPI.Omantel:                    
                    sendMessageOmantel(sms);
                    break;

                case SMSAPI.Ooredoo:
                    sendMessageOoredoo(sms);
                    break;

                default:
                    throw new ArgumentException("SMS API not found");
            }
        }

        public int Send(SMSMessage sms)
        {
            if (_isSMSEnabled)
            {
                sendSMS(sms);
                logUserinContext(_currentUsername);
                db.SMSLogs.Add(_smslog);
                return db.SaveChanges();
            }
            return 0;
        }

        public IEnumerable<SMSLog> GetLogs()
        {
            return db.SMSLogs.ToList();
        }

        public async Task<IEnumerable<SMSLog>> GetLogsAsync(DateTime from, DateTime to)
        {
            return await db.SMSLogs.Where(s => s.TimeSent > from && s.TimeSent < to ).ToListAsync();
        }

        private string[] getMobileNumbers(List<Mobile> Mobiles)
        {
            List<string> MobileNumbers = new List<string>();
            foreach (var item in Mobiles)
            {
                if (item.MobileNumber != null)
                {
                    if (item.Number.StartsWith("9") || item.Number.StartsWith("7"))
                        MobileNumbers.Add(item.MobileNumber);
                }
            }
            return MobileNumbers.ToArray();
        }

        private int sendMessageOmantel(SMSMessage sms)
        {
            int statusCode = 0;
            string[] MobileNumbers = getMobileNumbers(sms.Mobiles);
            if (sms.Message.Length > 0 || sms.Message.Length < 479 || MobileNumbers.Length > 0)
            {
                IBulkSMS a = new IBulkSMS();

                _smslog = new SMSLog()
                {
                    SMSAPI = sms.SMSAPI,
                    Message = sms.Message,
                    Language = sms.Language,
                    ScheddateTime = sms.ScheduleTime,
                    Recipients = string.Join(", ", MobileNumbers),
                    RecipientType = sms.RecipientType
                };

                statusCode = a.PushMessage(_smsconfig.UserID, _smsconfig.password, _smslog.Message, (int)_smslog.Language, DateTime.Now, MobileNumbers, (int)_smslog.RecipientType);

                if (Enum.IsDefined(typeof(OmantelSMSPushResult), statusCode))
                {
                    _smslog.SMSPushResult = statusCode;
                }
                else
                {
                    _smslog.SMSPushResult = (int)OmantelSMSPushResult.NewErrorCode;
                    _smslog.optValue = statusCode.ToString();
                }
            }
            return statusCode;
        }

        private int sendMessageOoredoo(SMSMessage sms)
        {
            int statusCode = 0;
            string[] MobileNumbers = getMobileNumbers(sms.Mobiles);
            if (sms.Message.Length > 0 || sms.Message.Length < 479 || MobileNumbers.Length > 0)
            {
                BulkPushSoapClient a = new BulkPushSoapClient();

                _smslog = new SMSLog()
                {
                    SMSAPI = sms.SMSAPI,
                    Message = sms.Message,
                    Language = sms.Language,
                    ScheddateTime = sms.ScheduleTime,
                    Recipients = string.Join(", ", MobileNumbers),
                    RecipientType = sms.RecipientType
                };

                var status = a.SendSMS(_smsconfig.UserID, _smsconfig.password, _smslog.Message, 1, null, "Aqua-Tots", null, "Daftari", string.Join(",", MobileNumbers));
                statusCode = int.Parse(status.StatusCode);
                if (Enum.IsDefined(typeof(OoredooSMSPushResult), statusCode))
                {
                    _smslog.SMSPushResult = statusCode;
                }
                else
                {
                    _smslog.SMSPushResult = (int)OoredooSMSPushResult.NewErrorCode;
                    _smslog.optValue = statusCode.ToString() + " " + status.StatusDesc;
                }
            }
            return statusCode;
        }

        private void logUserinContext(string User)
        {
            _smslog.AuditDetail.CreatedEntryUserID = User;
        }

        public string Result
        {
            get
            {
                string ErrorMessage = "Success";
                if (((int)_smslog.SMSPushResult > 1 && _smslog.SMSAPI == SMSAPI.Omantel) || ((int)_smslog.SMSPushResult > 0 && _smslog.SMSAPI == SMSAPI.Ooredoo))
                    ErrorMessage = "Error: " + _smslog.SMSStatus;

                return ErrorMessage;
            }
        }

        private void setSMSConfig(SMSAPI api)
        {
            if(_smsconfig == null || _smsconfig.SMSAPI != api)
            _smsconfig = db.SMSConfigs.Where(c => c.SMSAPI == api)
                .OrderByDescending(c => c.AuditDetail.CreatedDate)
                .FirstOrDefault();
        }

        public void Dispose()
        {
            db.Dispose();
            _smslog = null;
            _smsconfig = null;
        }
    }
}