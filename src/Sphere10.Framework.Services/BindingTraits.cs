//-----------------------------------------------------------------------
// <copyright file="BindingTraits.cs" company="Sphere 10 Software">
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
    [Flags]
    public enum BindingTraits : int {
        BasicHttp = 1 << 0,
        _Reserved1 = 1 << 1,
        _Reserved2 = 1 << 2,
        _Reserved3 = 1 << 3,
        _Reserved4 = 1 << 4,
        _Reserved5 = 1 << 5,
        _Reserved6 = 1 << 6,
        _Reserved7 = 1 << 7,
        _Reserved8 = 1 << 8,
        _Reserved9 = 1 << 9,
        _Reserved10 = 1 << 10,
        _Reserved11 = 1 << 11,
        _Reserved12 = 1 << 12,
        _Reserved13 = 1 << 13,
        _Reserved14 = 1 << 14,
        _Reserved15 = 1 << 15,
        _Reserved16 = 1 << 16,
        _Reserved17 = 1 << 17,
        _Reserved18 = 1 << 18,
        _Reserved19 = 1 << 19,
        _Reserved20 = 1 << 20,
        _Reserved21 = 1 << 21,
        _Reserved22 = 1 << 22,
        BufferedRequest = 1 << 23,
        BufferedResponse = 1 << 24,
        StreamedRequest = 1 << 25,
        StreamedResponse = 1 << 26,
        BufferedTraffic = BufferedRequest | BufferedResponse,
        StreamedTraffic = StreamedRequest | StreamedResponse,
        LargeRequests = 1 << 27,
        LargeResponses = 1 << 28,
        SlowRequests = 1 << 29,
        SlowResponses = 1 << 30,
        SlowOpenCloses = 1 << 31,
        HighlyCourseGrained = LargeRequests | LargeResponses,
        HighLag = SlowRequests | SlowResponses
    }
}
