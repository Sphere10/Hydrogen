//-----------------------------------------------------------------------
// <copyright file="IRichTextBox.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Windows.Forms;

namespace SourceGrid.Cells.Models
{
    /// <summary>
    /// Model for RichTextBox
    /// </summary>
    public interface IRichTextBox : IModel
    {
        /// <summary>
        /// Set font of selected text.
        /// </summary>
        /// <param name="cellContext"></param>
        /// <param name="font"></param>
        void SetSelectionFont(CellContext cellContext, Font font);

        /// <summary>
        /// Get font of current selection
        /// </summary>
        /// <param name="cellContext"></param>
        /// <returns></returns>
        Font GetSelectionFont(CellContext cellContext);

        /// <summary>
        /// Set color of selected text.
        /// </summary>
        void SetSelectionColor(CellContext cellContext, Color color);

        /// <summary>
        /// Get char offset of current selection
        /// </summary>
        /// <param name="cellContext"></param>
        /// <returns></returns>
        Color GetSelectionColor(CellContext cellContext);

        /// <summary>
        /// Set char offset of selected text.
        /// </summary>
        void SetSelectionCharOffset(CellContext cellContext, int charOffset);

        /// <summary>
        /// Get char offset of current selection
        /// </summary>
        /// <param name="cellContext"></param>
        /// <returns></returns>
        int GetSelectionCharOffset(CellContext cellContext);

        /// <summary>
        /// Set horizontal alignment of selected text.
        /// </summary>
        void SetSelectionAlignment(CellContext cellContext, HorizontalAlignment horAlignment);

        /// <summary>
        /// Get horizontal alignment of current selection
        /// </summary>
        /// <param name="cellContext"></param>
        /// <returns></returns>
        HorizontalAlignment GetSelectionAlignment(CellContext cellContext);
    }
}
