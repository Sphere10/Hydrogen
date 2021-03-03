namespace Sphere10.Helium.Saga
{
    public abstract class Saga<TSagaData> : Saga where TSagaData : ISagaDataForSaga, new()
    {
        public TSagaData Data
        {
            get => (TSagaData)Entity;
            set => Entity = (ISagaDataForSaga)value;
        }

        protected internal override void ConfigureHowToFindSaga(IFindSaga sagaFindMap)
        {
            ConfigureHowToFindSaga(new SagaPropertyMapper<TSagaData>(sagaFindMap));
        }

        protected abstract void ConfigureHowToFindSaga(SagaPropertyMapper<TSagaData> mapper);
    }
}
