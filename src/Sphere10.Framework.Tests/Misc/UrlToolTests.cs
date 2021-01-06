//-----------------------------------------------------------------------
// <copyright file="UrlToolTests.cs" company="Sphere 10 Software">
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

using NUnit.Framework;

namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class UrlToolTests {


        [Test]
        public void ParseQueryString_WithJSON() {
            Assert.DoesNotThrow(() => Tools.Url.ParseQueryString("&siteContext=1&actionName=SqlSelect&actionParameters=%7B%22sql%22%3A+%22SELECT+%09%09%09%09%09%09FT.ID+AS+FormTypeID%2C+%09%09%09%09%09%09FC.Name+AS+FormCategoryName%2C+%09%09%09%09%09%09FT.Name+AS+FormTypeName%2C+%09%09%09%09%09%09FT.ImageBlobID+AS+PinBlobID+%09%09%09%09%09FROM+%09%09%09%09%09%09FormType+FT+INNER+JOIN+%09%09%09%09%09%09FormCategory+FC+ON+FT.FormCategoryID+%3D+FC.ID+%09%09%09%09%09ORDER+BY+%09%09%09%09%09%09FC.Name%2C+FT.Name%22%7D"));
        }

    }

}
