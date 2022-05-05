//-----------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Sphere 10 Software">
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

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Summary description for Utilities.
	/// </summary>
	public class Utilities
	{
		public Utilities()
		{
		}

		public static System.Windows.Forms.HorizontalAlignment ContentToHorizontalAlignment(DevAge.Drawing.ContentAlignment a)
		{
			if (Drawing.Utilities.IsLeft(a))
				return System.Windows.Forms.HorizontalAlignment.Left;
			else if (Drawing.Utilities.IsRight(a))
				return System.Windows.Forms.HorizontalAlignment.Right;
			else
				return System.Windows.Forms.HorizontalAlignment.Center;
		}

        public static System.Windows.Forms.TextFormatFlags ContentAligmentToTextFormatFlags(DevAge.Drawing.ContentAlignment a)
        {
            System.Windows.Forms.TextFormatFlags f = (System.Windows.Forms.TextFormatFlags)0;

            if (Drawing.Utilities.IsBottom(a))
                f |= System.Windows.Forms.TextFormatFlags.Bottom;
            else if (Drawing.Utilities.IsTop(a))
                f |= System.Windows.Forms.TextFormatFlags.Top;
            else //if (Drawing.Utilities.IsMiddle(a))
                f |= System.Windows.Forms.TextFormatFlags.VerticalCenter;


            if (Drawing.Utilities.IsLeft(a))
                f |= System.Windows.Forms.TextFormatFlags.Left;
            else if (Drawing.Utilities.IsRight(a))
                f |= System.Windows.Forms.TextFormatFlags.Right;
            else //if (Drawing.Utilities.IsCenter(a))
                f |= System.Windows.Forms.TextFormatFlags.HorizontalCenter;

            return f;
        }
	}
}
