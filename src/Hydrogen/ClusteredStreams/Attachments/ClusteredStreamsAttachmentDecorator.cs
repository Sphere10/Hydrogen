namespace Hydrogen;

public class ClusteredStreamsAttachmentDecorator<TInner> : IClusteredStreamsAttachment 
	where TInner : IClusteredStreamsAttachment {

	protected readonly TInner Inner;

	protected ClusteredStreamsAttachmentDecorator(TInner innerAttachment) {
		Inner = innerAttachment;
	}

	public virtual string AttachmentID => Inner.AttachmentID;

	public virtual ClusteredStreams Streams => Inner.Streams;

	public virtual bool IsAttached => Inner.IsAttached;

	public virtual void Attach() => Inner.Attach();

	public virtual void Detach() => Inner.Detach();

	public virtual void Flush() => Inner.Flush();
}


public class ClusteredStreamsAttachmentDecorator : ClusteredStreamsAttachmentDecorator<IClusteredStreamsAttachment> {

	protected ClusteredStreamsAttachmentDecorator(IClusteredStreamsAttachment innerAttachment) 
		: base(innerAttachment) {
	}
}
