// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;

namespace Hydrogen;

public class DefaultDateAttribute : DefaultValueAttribute {

	public DefaultDateAttribute(string dateTimeString) : base(DateTime.Parse(dateTimeString)) {
	}

	public DefaultDateAttribute(bool useUTC, int nowYearOffset, int nowMonthOffset, int nowDayOffset, int nowHourOffset, int nowMinuteOffset, int nowSecondOffset, int nowMillisecondOffset)
		: base((useUTC ? DateTime.UtcNow : DateTime.Now).AddYears(nowYearOffset).AddMonths(nowMonthOffset).AddDays(nowDayOffset).AddMinutes(nowMinuteOffset).AddSeconds(nowDayOffset).AddMilliseconds(nowMillisecondOffset)) {
	}

}
