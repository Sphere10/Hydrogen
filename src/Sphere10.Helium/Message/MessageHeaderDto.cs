using System;

namespace Sphere10.Helium.Message {
	public record MessageHeaderDto {
		string Id { get; init; }

		string ReplyToAddress { get; init; }

		TimeSpan? TimeToLive { get; init; }

		DateTime? DateToDie { get; init; }

		public MessageHeaderDto(string id, string replyToAddress, TimeSpan? timeToLive, DateTime? dateToDie) {
			Id = id;
			ReplyToAddress = replyToAddress;
			TimeToLive = timeToLive;
			DateToDie = dateToDie;
		}
	}
}
