//-----------------------------------------------------------------------
// <copyright file="FormSingleton.cs" company="Sphere 10 Software">
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
	public class FormSingleton
	{
		private Type m_FormType;
		private object[] m_Args;
		public FormSingleton(Type p_FormType, object[] p_Args)
		{
			m_FormType = p_FormType;
			m_Args = p_Args;
		}

		
		private System.Windows.Forms.Form m_Form = null;
		public System.Windows.Forms.Form GetForm()
		{
			if (m_Form==null)
			{
				m_Form = (System.Windows.Forms.Form)Activator.CreateInstance(m_FormType,m_Args);

				m_Form.CreateControl();

				m_Form.Closed +=new EventHandler(m_Form_Closed);
			}

			return m_Form;
		}

		public bool IsFormCreated
		{
			get{return (m_Form != null);}
		}

		private void m_Form_Closed(object sender, EventArgs e)
		{
			m_Form.Closed -=new EventHandler(m_Form_Closed);
			m_Form = null;
		}
	}
}
