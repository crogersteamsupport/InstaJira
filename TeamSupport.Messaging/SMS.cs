using System;
using System.Collections.Generic;
using System.Net;
using Twilio;

namespace TeamSupport.Messaging
{
	public class SMS
	{
		private string _teamSupportTwilioNumber = string.Empty;
		private string _accountSid			= string.Empty;
		private string _authToken			= string.Empty;
		private string _messageId			= string.Empty;
		private string _text					= string.Empty;
		private string _destinationNumber = string.Empty;
		private bool	_isSuccessful		= false;
		private bool	_hasBeenValidated = false;
		private bool	_isValid				= false;
		private List<SMSError> _smsErrors = new List<SMSError>();

		public SMS()
		{
			_accountSid = System.Web.Configuration.WebConfigurationManager.AppSettings["TwilioAccountSid"];
			_authToken = System.Web.Configuration.WebConfigurationManager.AppSettings["TwilioAuthToken"];
			_teamSupportTwilioNumber = System.Web.Configuration.WebConfigurationManager.AppSettings["TwilioNumber"];

			_messageId			= string.Empty;
			_isSuccessful		= false;
			_hasBeenValidated = false;
			_isValid				= false;
			_smsErrors			= new List<SMSError>();
		}

		public void Send(string text, string number)
		{
			SMSText = text;
			DestinationNumber = number;

			if (IsValid)
			{
				try
				{
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
					var twilio = new TwilioRestClient(_accountSid, _authToken);
					var message = twilio.SendMessage(_teamSupportTwilioNumber, DestinationNumber, SMSText);

					if (message.RestException != null)
					{
						SMSError smsError = new SMSError()
						{
							Code = message.RestException.Code,
							Message = message.RestException.Message,
							Status = message.RestException.Status,
							MoreInfo = message.RestException.MoreInfo
						};

						_smsErrors.Add(smsError);
					}
					else
					{
						_messageId = message.Sid;
					}
				}
				catch (Exception ex)
				{
					SMSError smsError = new SMSError()
					{
						Code = "0",
						Message = ex.Message,
						Status = "0",
						MoreInfo = ex.StackTrace
					};

					_smsErrors.Add(smsError);
				}
			}

			IsSuccessful = SMSErrors.Count == 0 && !string.IsNullOrEmpty(MessageId);
		}

		private void Validate()
		{
			_isValid = true;
			_smsErrors = new List<SMSError>();

			if (string.IsNullOrEmpty(_destinationNumber))
			{
				SMSError smsError = new SMSError()
				{
					Code = "0",
					Message = "Need a number to send the message to.",
					Status = "0",
					MoreInfo = "Validation Error"
				};

				_smsErrors.Add(smsError);
			}

			if (string.IsNullOrEmpty(_text))
			{
				SMSError smsError = new SMSError()
				{
					Code = "0",
					Message = "Need a text to send.",
					Status = "0",
					MoreInfo = "Validation Error"
				};

				_smsErrors.Add(smsError);
			}

			_hasBeenValidated = true;
			_isValid = _smsErrors.Count == 0;
		}

		#region Properties

		private bool IsValid
		{
			get
			{
				if (!_hasBeenValidated)
				{
					Validate();
				}

				return _isValid;
			}
		}

		public bool IsSuccessful
		{
			get
			{
				return _isSuccessful;
			}
			private set
			{
				_isSuccessful = value;
			}
		}

		public string MessageId
		{
			get
			{
				return _messageId;
			}
			private set
			{
				_messageId = value;
			}
		}

		private string SMSText
		{
			set
			{
				_text = value;
				_hasBeenValidated = false;
			}
			get
			{
				return _text;
			}
		}

		private string DestinationNumber
		{
			set
			{
				_destinationNumber = value;
				_hasBeenValidated = false;
			}
			get
			{
				return _destinationNumber;
			}
		}

		public List<SMSError> SMSErrors
		{
			get
			{
				return _smsErrors;
			}
		}

		#endregion

		public class SMSError
		{
			private string _code;
			private string _status;
			private string _message;			
			private string _moreInfo;

			#region Properties

			public string Code
			{
				get
				{
					return _code;
				}
				set
				{
					_code = value;
				}
			}

			public string Status
			{
				get
				{
					return _status;
				}
				set
				{
					_status = value;
				}
			}

			public string Message
			{
				get
				{
					return _message;
				}
				set
				{
					_message = value;
				}
			}

			public string MoreInfo
			{
				get
				{
					return _moreInfo;
				}
				set
				{
					_moreInfo = value;
				}
			}

			#endregion
		}
	}
}
