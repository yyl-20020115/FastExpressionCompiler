using System;
using NUnit.Framework;

#if !LIGHT_EXPRESSION
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
namespace FastExpressionCompiler.UnitTests
{
    [TestFixture]
    public class ValueTypeTests : ITest
    {
        public int Run()
        {
            Should_support_struct_params_with_field_access();
            Should_support_virtual_calls_on_struct_arguments();
            Should_support_virtual_calls_with_parameters_on_struct_arguments();
            Can_create_struct();
            Can_init_struct_member();
            Can_get_struct_member();
            Action_using_with_struct_closure_field();
            Struct_Convert_to_interface();
            return 8;
        }

        [Test]
        public void Should_support_struct_params_with_field_access()
        {
            Expression<Func<StructA, int>> expr = a => a.N;

            var f = expr.CompileFast(true);

            Assert.AreEqual(42, f(new StructA { N = 42 }));
        }

        [Test]
        public void Should_support_virtual_calls_on_struct_arguments()
        {
            Expression<Func<StructA, string>> expr = a => a.ToString();

            var f = expr.CompileFast(true);

            Assert.AreEqual("42", f(new StructA { N = 42 }));
        }

        [Test]
        public void Should_support_virtual_calls_with_parameters_on_struct_arguments()
        {
            object aa = new StructA();
            Expression<Func<StructA, bool>> expr = a => a.Equals(aa);

            var f = expr.CompileFast(true);

            Assert.AreEqual(false, f(new StructA { N = 42 }));
        }

        [Test]
        public void Can_create_struct()
        {
            Expression<Func<StructA>> expr = () => new StructA();

            var newA = expr.CompileFast<Func<StructA>>(true);

            Assert.AreEqual(0, newA().N);
        }

        [Test]
        public void Can_init_struct_member()
        {
            Expression<Func<StructA>> expr = () => new StructA { N = 43, M = 34, Sf = "sf", Sp = "sp" };

            var newA = expr.CompileFast<Func<StructA>>(true);

            var a = newA();
            Assert.AreEqual(43, a.N);
            Assert.AreEqual(34, a.M);
            Assert.AreEqual("sf", a.Sf);
            Assert.AreEqual("sp", a.Sp);
        }

        [Test]
        public void Can_get_struct_member()
        {
            Expression<Func<int>> exprN = () => new StructA { N = 43, M = 34, Sf = "sf", Sp = "sp" }.N;
            Expression<Func<int>> exprM = () => new StructA { N = 43, M = 34, Sf = "sf", Sp = "sp" }.M;
            Expression<Func<string>> exprSf = () => new StructA { N = 43, M = 34, Sf = "sf", Sp = "sp" }.Sf;
            Expression<Func<string>> exprSp = () => new StructA { N = 43, M = 34, Sf = "sf", Sp = "sp" }.Sp;


            var n = exprN.CompileFast<Func<int>>(true);
            var m = exprM.CompileFast<Func<int>>(true);
            var sf = exprSf.CompileFast<Func<string>>(true);
            var sp = exprSp.CompileFast<Func<string>>(true);

            Assert.AreEqual(43, n());
            Assert.AreEqual(34, m());
            Assert.AreEqual("sf", sf());
            Assert.AreEqual("sp", sp());
        }

        struct StructA
        {
            public int N;
            public int M { get; set; }
            public string Sf;
            public string Sp { get; set; }

            public override string ToString() => N.ToString();
        }

        [Test]
        public void Action_using_with_struct_closure_field()
        {
            var s = new SS();
            Expression<Action<string>> expr = a => s.SetValue(a);

            var lambda = expr.CompileFast(ifFastFailedReturnNull: true);
            lambda("a");
            Assert.AreEqual("a", s.Value);
        }

        [Test]
        public void Struct_Convert_to_interface()
        {
            Expression<Func<int, IComparable>> expr = a => a;
            Expression<Func<DateTimeKind, IComparable>> expr2 = a => a;
            Expression<Func<SS, IDisposable>> expr3 = a => a;

            Assert.AreEqual(12, expr.CompileFast(ifFastFailedReturnNull: true)(12));
            Assert.AreEqual(DateTimeKind.Local, expr2.CompileFast(ifFastFailedReturnNull: true)(DateTimeKind.Local));
            Assert.AreEqual(new SS { Value = "a" }, expr3.CompileFast(ifFastFailedReturnNull: true)(new SS { Value = "a" }));
        }

        public struct SS: IDisposable
        {
            public string Value;

            public void SetValue(string s)
            {
                Value = s;
            }
            public void Dispose() { }
        }
    }
}
#endif