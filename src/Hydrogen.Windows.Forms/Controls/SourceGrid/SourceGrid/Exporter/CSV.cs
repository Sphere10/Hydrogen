//-----------------------------------------------------------------------
// <copyright file="CSV.cs" company="Sphere 10 Software">
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

namespace SourceGrid.Exporter
{
    /// <summary>
    /// An utility class to export a grid to a csv delimited format file.
    /// </summary>
    public class CSV
    {
        public CSV()
        {
            mFieldSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            mLineSeparator = System.Environment.NewLine;
        }

        private string mFieldSeparator;
        public string FieldSeparator
        {
            get { return mFieldSeparator; }
            set { mFieldSeparator = value; }
        }
        private string mLineSeparator;
        public string LineSeparator
        {
            get { return mLineSeparator; }
            set { mLineSeparator = value; }
        }

        public virtual void Export(GridVirtual grid, System.IO.TextWriter stream)
        {
            for (int r = 0; r < grid.Rows.Count; r++)
            {
                for (int c = 0; c < grid.Columns.Count; c++)
                {
                    if (c > 0)
                        stream.Write(mFieldSeparator);

                    Cells.ICellVirtual cell = grid.GetCell(r, c);
                    Position pos = new Position(r, c);
                    CellContext context = new CellContext(grid, pos, cell);
                    ExportCSVCell(context, stream);
                }
                stream.Write(mLineSeparator);
            }
        }

        protected virtual void ExportCSVCell(CellContext context, System.IO.TextWriter stream)
        {
            if (context.Cell != null)
            {
                string text = context.DisplayText;
                if (text == null)
                    text = string.Empty;
                text = text.Replace("\r\n", " ");
                text = text.Replace("\n", " ");
                text = text.Replace("\r", " ");
                stream.Write(text);
            }
            else
            {
                stream.Write("");
            }
        }
    }

}
