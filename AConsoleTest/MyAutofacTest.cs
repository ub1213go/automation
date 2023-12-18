using AConsole.MyAutofac;
namespace AConsoleTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var auto = new Autofac();
            auto.Register<IA>(typeof(A));
            auto.Register(typeof(B));
            var b = auto.Get<B>();
            Assert.That(b.Say(), Is.EqualTo("I'm A"));
        }

        [Test]
        public void Test2()
        {
            var auto = new Autofac();
            auto.Register<IA>(typeof(A2));
            auto.Register(typeof(B));
            var b = auto.Get<B>();
            Assert.That(b.Say(), Is.EqualTo("I'm A2"));
        }

        public interface IA
        {
            string Say();
        }

        public class A : IA
        {
            public string Say()
            {
                return "I'm A";
            }
        }

        public class A2 : IA
        {
            public string Say()
            {
                return "I'm A2";
            }
        }

        public class B
        {
            public IA a { get; set; }
            public B(IA a)
            {
                this.a = a;
            }

            public string Say()
            {
                return a.Say();
            }
        }
    }
}