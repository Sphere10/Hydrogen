using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sphere10.Framework.Windows.Forms {
	public partial class QuestionDialog : DialogEx {

		public QuestionDialog() : this(SystemIconType.None, string.Empty, string.Empty, "OK") {
		}

		public QuestionDialog(SystemIconType iconType, string title, string text, params string[] buttonNames) 
			: base(iconType, title, text, true, buttonNames) {
		}



	}
}
