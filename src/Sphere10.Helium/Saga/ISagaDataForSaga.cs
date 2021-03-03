using System;

namespace Sphere10.Helium.Saga
{
    public interface ISagaDataForSaga
    {
        Guid Id { get; set; }

        string Originator { get; set; }

        string OriginalMessageId { get; set; }
    }
}
