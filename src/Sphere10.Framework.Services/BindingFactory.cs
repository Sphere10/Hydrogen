//-----------------------------------------------------------------------
// <copyright file="BindingFactory.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace Sphere10.Framework.Services {
	public static class BindingFactory {
        private const TransferMode TransferMode = System.ServiceModel.TransferMode.Buffered;

        private const int StandardMaxBufferSize = 65536;
        private const int StandardMaxBufferPoolSize = 65536;
        private const int StandardMaxReceivedMessageSize = 65536;
        private const int StandardOpenTimeoutMin = 1;
        private const int StandardSendTimeoutMin = 1;
        private const int StandardReceiveTimeoutMin = 10;
        private const int StandardCloseTimeoutMin = 1;

        private const int LargeMaxBufferSize = 2147483647;
        private const int LargeMaxBufferPoolSize = 2147483647;
        private const int LargeMaxReceivedMessageSize = 2147483647;
	    private const int LongOpenTimeoutMin = 10;
        private const int LongSendTimeoutMin = 30;
        private const int LongReceiveTimeoutMin = 30;
        private const int LongCloseTimeoutMin = 10;

        private const int StandardSoapMaxStringContentLength = 8192;
        private const int StandardSoapMaxArrayLength = 16384;
        private const int StandardSoapMaxBytesPerRead = 4096;
        private const int StandardSoapMaxDepth = 32;
        private const int StandardSoapMaxNameTableCharCount = 16384;

        private const int LargeSoapMaxStringContentLength = 2147483647;
        private const int LargeSoapMaxArrayLength = 2147483647;
        private const int LargeSoapMaxBytesPerRead = 2147483647;
        private const int LargeSoapMaxDepth = 2147483647;
        private const int LargeSoapMaxNameTableCharCount = 2147483647;

        private const TransferMode DefaultTransferMode = System.ServiceModel.TransferMode.Buffered;
		
        public static Binding CreateStandardBinding() {
		    return CreateBinding(BindingTraits.BasicHttp | BindingTraits.BufferedTraffic);
		}

	    public static Binding CreateBinding(BindingTraits bindingTraits) {
	        if (bindingTraits.HasFlag(BindingTraits.BasicHttp)) {
	            return CreateBasicHttpBinding(bindingTraits);
	        } 
            throw new NotSupportedException(bindingTraits.ToString());
	    }

        private static Binding CreateBasicHttpBinding(BindingTraits bindingTraits) {
            BasicHttpSecurityMode securityMode;
            // TODO: determine security mode from traits
            securityMode = BasicHttpSecurityMode.None;
            var binding = new BasicHttpBinding(securityMode);

            binding.TransferMode = DetermineTransferMode(bindingTraits);
            var largeRequests = bindingTraits.HasFlag(BindingTraits.LargeRequests);
            var largeResponses = bindingTraits.HasFlag(BindingTraits.LargeResponses);
            var longRequests = bindingTraits.HasFlag(BindingTraits.SlowRequests);
            var longResponses = bindingTraits.HasFlag(BindingTraits.SlowResponses);

            binding.MaxBufferSize = largeRequests ? LargeMaxBufferSize : StandardMaxBufferSize;
            binding.MaxBufferPoolSize = largeRequests ? LargeMaxBufferPoolSize : StandardMaxBufferPoolSize;
            binding.MaxReceivedMessageSize = largeResponses ? LargeMaxReceivedMessageSize : StandardMaxReceivedMessageSize;
            binding.OpenTimeout = longResponses ? TimeSpan.FromMinutes(LongOpenTimeoutMin) : TimeSpan.FromMinutes(StandardOpenTimeoutMin);
            binding.SendTimeout = longRequests ? TimeSpan.FromMinutes(LongSendTimeoutMin) : TimeSpan.FromMinutes(StandardSendTimeoutMin);
            binding.ReceiveTimeout = longResponses ? TimeSpan.FromMinutes(LongReceiveTimeoutMin) : TimeSpan.FromMinutes(StandardReceiveTimeoutMin);
            binding.CloseTimeout = longResponses ? TimeSpan.FromMinutes(LongCloseTimeoutMin) : TimeSpan.FromMinutes(StandardCloseTimeoutMin);
            binding.ReaderQuotas = (largeRequests || largeResponses) ? CreateLargeSoapReader() : CreateStandardSoapReader();
            return binding;
        }

        private static TransferMode DetermineTransferMode(BindingTraits traits) {
	        if (traits.HasFlag(BindingTraits.BufferedTraffic))
				return System.ServiceModel.TransferMode.Buffered;

	        if (traits.HasFlag(BindingTraits.StreamedTraffic))
				return System.ServiceModel.TransferMode.Streamed;

	        if (traits.HasFlag(BindingTraits.StreamedRequest)) {
				return System.ServiceModel.TransferMode.StreamedRequest;
	        }

	        if (traits.HasFlag(BindingTraits.StreamedResponse)) {
				return System.ServiceModel.TransferMode.StreamedResponse;
	        }

	        return DefaultTransferMode;

	    }

	    private static XmlDictionaryReaderQuotas CreateLargeSoapReader() {
            return new XmlDictionaryReaderQuotas {
                MaxStringContentLength = LargeSoapMaxStringContentLength,
                MaxArrayLength = LargeSoapMaxArrayLength,
                MaxBytesPerRead = LargeSoapMaxBytesPerRead,
                MaxDepth = LargeSoapMaxDepth,
                MaxNameTableCharCount =  LargeSoapMaxNameTableCharCount
            };
	    }

        private static XmlDictionaryReaderQuotas CreateStandardSoapReader() {
            return new XmlDictionaryReaderQuotas {
                MaxStringContentLength = StandardSoapMaxStringContentLength,
                MaxArrayLength = StandardSoapMaxArrayLength,
                MaxBytesPerRead = StandardSoapMaxBytesPerRead,
                MaxDepth = StandardSoapMaxDepth,
                MaxNameTableCharCount = StandardSoapMaxNameTableCharCount
            };
	    }

	}
}
