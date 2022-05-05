namespace AbstractProtocol.AnonymousPipeComplex {
    public enum AppProtocolMessageType {
        Sync,
        Ack,
        Verack,
        Ping,
        Pong,
        RequestListFolder,
        FileDescriptor,
        DirectoryDescriptor,
        FolderContents,
        RequestFilePart,
        FilePart,
        NotifyLayer2Message,
        NotifyNewBlock,
        NotifyNewTransaction,
    }
}
