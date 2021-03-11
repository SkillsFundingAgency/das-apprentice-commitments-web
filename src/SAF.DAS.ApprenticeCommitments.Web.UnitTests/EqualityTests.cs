// https://codinghelmet.com/articles/testing-equals-and-gethashcode
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{
    public static class EqualityTests
    {
        private struct TestResult
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }

            public static TestResult CreateSuccess()
            {
                return new TestResult()
                {
                    IsSuccess = true
                };
            }

            public static TestResult CreateFailure(string message)
            {
                return new TestResult()
                {
                    IsSuccess = false,
                    ErrorMessage = message
                };
            }
        }

        public static void TestEqualObjects<T>(T obj1, T obj2)
        {
            ThrowIfAnyIsNull(obj1, obj2);

            IList<TestResult> testResults = new List<TestResult>()
            {
                TestGetHashCodeOnEqualObjects<T>(obj1, obj2),
                TestEquals<T>(obj1, obj2, true),
                TestEqualsOfT<T>(obj1, obj2, true),
                TestEqualityOperator<T>(obj1, obj2, true),
                TestInequalityOperator<T>(obj1, obj2, false)
            };

            AssertAllTestsHavePassed(testResults);
        }

        public static void TestUnequalObjects<T>(T obj1, T obj2)
        {
            ThrowIfAnyIsNull(obj1, obj2);

            IList<TestResult> testResults = new List<TestResult>()
            {
                TestEqualsReceivingNonNullOfOtherType<T>(obj1),
                TestEquals<T>(obj1, obj2, false),
                TestEqualsOfT<T>(obj1, obj2, false),
                TestEqualityOperator<T>(obj1, obj2, false),
                TestInequalityOperator<T>(obj1, obj2, true)
            };

            AssertAllTestsHavePassed(testResults);
        }

        public static void TestAgainstNull<T>(T obj)
        {
            ThrowIfAnyIsNull(obj);

            IList<TestResult> testResults = new List<TestResult>()
            {
                TestEqualsReceivingNull<T>(obj),
                TestEqualsOfTReceivingNull<T>(obj),
                TestEqualityOperatorReceivingNull<T>(obj),
                TestInequalityOperatorReceivingNull<T>(obj),
            };

            AssertAllTestsHavePassed(testResults);
        }

        private static TestResult
            TestGetHashCodeOnEqualObjects<T>(T obj1, T obj2)
        {
            return SafeCall("GetHashCode", () =>
            {
                if (obj1.GetHashCode() != obj2.GetHashCode())
                    return TestResult.CreateFailure(
                        "GetHashCode of equal objects " +
                        "returned different values.");
                return TestResult.CreateSuccess();
            });
        }

        private static TestResult
            TestEqualsReceivingNonNullOfOtherType<T>(T obj)
        {
            return SafeCall("Equals", () =>
            {
                if (obj.Equals(new object()))
                    return TestResult.CreateFailure(
                        "Equals returned true when comparing " +
                        "with object of a different type.");
                return TestResult.CreateSuccess();
            });
        }

        private static TestResult TestEqualsReceivingNull<T>(T obj)
        {
            if (typeof(T).IsClass)
                return TestEquals<T>(obj, default(T), false);
            return TestResult.CreateSuccess();
        }

        private static TestResult
            TestEqualsOfTReceivingNull<T>(T obj)
        {
            if (typeof(T).IsClass)
                return TestEqualsOfT<T>(obj, default(T), false);
            return TestResult.CreateSuccess();
        }

        private static TestResult TestEquals<T>(T obj1, T obj2,
                                                bool expectedEqual)
        {
            return SafeCall("Equals", () =>
            {
                if (obj1.Equals((object)obj2) != expectedEqual)
                {
                    string message =
                        string.Format("Equals returns {0} " +
                                      "on {1}equal objects.",
                                      !expectedEqual,
                                      expectedEqual ? "" : "non-");
                    return TestResult.CreateFailure(message);
                }
                return TestResult.CreateSuccess();
            });
        }

        private static TestResult TestEqualsOfT<T>(T obj1, T obj2,
                                                   bool expectedEqual)
        {
            if (obj1 is IEquatable<T>)
                return TestEqualsOfTOnIEquatable<T>(obj1 as IEquatable<T>,
                                                    obj2, expectedEqual);
            return TestResult.CreateSuccess();
        }

        private static TestResult
            TestEqualsOfTOnIEquatable<T>(IEquatable<T> obj1, T obj2,
                                         bool expectedEqual)
        {
            return SafeCall("Strongly typed Equals", () =>
            {
                if (obj1.Equals(obj2) != expectedEqual)
                {
                    string message =
                        string.Format("Strongly typed Equals " +
                                      "returns {0} on {1}equal " +
                                      "objects.",
                                      !expectedEqual,
                                      expectedEqual ? "" : "non-");
                    return TestResult.CreateFailure(message);
                }
                return TestResult.CreateSuccess();
            });
        }

        private static TestResult
            TestEqualityOperatorReceivingNull<T>(T obj)
        {
            if (typeof(T).IsClass)
                return TestEqualityOperator<T>(obj, default(T), false);
            return TestResult.CreateSuccess();
        }

        private static TestResult
            TestEqualityOperator<T>(T obj1, T obj2, bool expectedEqual)
        {
            MethodInfo equalityOperator = GetEqualityOperator<T>();
            if (equalityOperator == null)
                return TestResult.CreateFailure("Type does not override " +
                                                "equality operator.");
            return TestEqualityOperator<T>(obj1, obj2, expectedEqual,
                                           equalityOperator);
        }

        private static TestResult
            TestEqualityOperator<T>(T obj1, T obj2, bool expectedEqual,
                                    MethodInfo equalityOperator)
        {
            return SafeCall("Operator ==", () =>
            {
                bool equal =
                    (bool)equalityOperator.Invoke(null,
                                                  new object[]
                                                  { obj1, obj2 });
                if (equal != expectedEqual)
                {
                    string message =
                        string.Format("Equality operator returned " +
                                      "{0} on {1}equal objects.",
                                      equal,
                                      expectedEqual ? "" : "non-");
                    return TestResult.CreateFailure(message);
                }
                return TestResult.CreateSuccess();
            });
        }

        private static TestResult
            TestInequalityOperatorReceivingNull<T>(T obj)
        {
            if (typeof(T).IsClass)
                return TestInequalityOperator<T>(obj, default(T), true);
            return TestResult.CreateSuccess();
        }

        private static TestResult
            TestInequalityOperator<T>(T obj1, T obj2,
                                      bool expectedUnequal)
        {
            MethodInfo inequalityOperator = GetInequalityOperator<T>();
            if (inequalityOperator == null)
                return TestResult.CreateFailure("Type does not override " +
                                                "inequality operator.");
            return TestInequalityOperator<T>(obj1, obj2, expectedUnequal,
                                             inequalityOperator);
        }

        private static TestResult
            TestInequalityOperator<T>(T obj1, T obj2,
                                      bool expectedUnequal,
                                      MethodInfo inequalityOperator)
        {
            return SafeCall("Operator !=", () =>
            {
                bool unequal =
                    (bool)inequalityOperator.Invoke(null,
                                                    new object[]
                                                    { obj1, obj2 });
                if (unequal != expectedUnequal)
                {
                    string message =
                        string.Format("Inequality operator retrned " +
                                      "{0} when comparing {1}equal " +
                                      "objects.",
                                      unequal,
                                      expectedUnequal ? "non-" : "");
                    return TestResult.CreateFailure(message);
                }
                return TestResult.CreateSuccess();
            });
        }

        private static void ThrowIfAnyIsNull(params object[] objects)
        {
            if (objects.Any(o => object.ReferenceEquals(o, null)))
                throw new System.ArgumentNullException();
        }

        private static TestResult SafeCall(string functionName,
                                           Func<TestResult> test)
        {
            try
            {
                return test();
            }
            catch (System.Exception ex)
            {
                string message =
                    string.Format("{0} threw {1}: {2}",
                                  functionName,
                                  ex.GetType().Name,
                                  ex.Message);

                return TestResult.CreateFailure(message);
            }
        }

        private static MethodInfo GetEqualityOperator<T>()
        {
            return GetOperator<T>("op_Equality");
        }

        private static MethodInfo GetInequalityOperator<T>()
        {
            return GetOperator<T>("op_Inequality");
        }

        private static MethodInfo GetOperator<T>(string methodName)
        {
            BindingFlags bindingFlags =
                BindingFlags.Static |
                BindingFlags.Public;
            MethodInfo equalityOperator =
                typeof(T).GetMethod(methodName, bindingFlags);
            return equalityOperator;
        }

        private static void
            AssertAllTestsHavePassed(IList<TestResult> testResults)
        {
            bool allTestsPass =
                testResults
                .All(r => r.IsSuccess);
            string[] errors =
                testResults
                .Where(r => !r.IsSuccess)
                .Select(r => r.ErrorMessage)
                .ToArray();
            string compoundMessage =
                string.Join(Environment.NewLine, errors);

            Assert.IsTrue(allTestsPass,
                          "Some tests have failed:\n" +
                          compoundMessage);
        }
    }
}