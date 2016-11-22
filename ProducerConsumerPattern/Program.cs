namespace ProducerConsumerPattern
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    internal static class Program
    {
        #region Methods

        private static async void AsynchronousConsumer(ISourceBlock<IList<int>> sourceBlock)
        {
            while (await sourceBlock.OutputAvailableAsync())
            {
                var producedResult = sourceBlock.Receive();
                foreach (var result in producedResult)
                {
                    Console.WriteLine("Receiver Received:" + result);
                }
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Running Test For: Producer Consumer");
            TestProducerConsumerFunction();
            Console.ReadKey();
        }

        private static void TestProducerConsumerFunction()
        {
            var sharedPayload = new BufferBlock<IList<int>>();
            WorkTaskComposer(sharedPayload);
            AsynchronousConsumer(sharedPayload);
        }

        private static async void WorkTaskComposer(ITargetBlock<IList<int>> targetBlock)
        {
            await Task.Run(
                () =>
                    {
                        var randomInteger = new Random();
                        while (true)
                        {
                            var list = new List<int>();

                            ////Do some work here to produce work for consumer.
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                            for (var generatorCounter = 0; generatorCounter < 4; generatorCounter++)
                            {
                                var value = randomInteger.Next(0, 100000);
                                Console.WriteLine("Producer Produced: " + value);
                                list.Add(value);
                            }

                            targetBlock.Post(list);
                        }
                    });
        }

        #endregion
    }
}