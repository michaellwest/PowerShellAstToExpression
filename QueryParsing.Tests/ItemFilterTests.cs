using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation.Language;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueryParsing.Tests
{
    [TestClass]
    public class ItemFilterTests
    {
        [TestMethod]
        public void Filter_List_By_Name_Equals()
        {
            //Arrange
            const string testName = "Michael";
            var names = new List<Person>
            {
                new Person {Name = testName},
                new Person {Name = "John"}
            }.AsQueryable();

            var predicate = ItemFilter<Person>(TokenKind.Equals, "item", "Name", testName);

            //Act
            var filtered = names.Where(predicate).ToList();

            //Assert
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual(testName, filtered[0].Name);
        }

        [TestMethod]
        public void Filter_List_By_Age_Equals()
        {
            //Arrange
            const int testAge = 31;
            var names = new List<Person>
            {
                new Person {Name = "Michael", Age = 31},
                new Person {Name = "John", Age = 40}
            }.AsQueryable();

            var predicate = ItemFilter<Person>(TokenKind.Equals, "item", "Age", testAge);

            //Act
            var filtered = names.Where(predicate).ToList();

            //Assert
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual(testAge, filtered[0].Age);
        }

        [TestMethod]
        public void Filter_List_By_Name_And_Age_Equals()
        {
            //Arrange
            const string testName = "Michael";
            const int testAge = 31;
            var names = new List<Person>
            {
                new Person {Name = "Michael", Age = 31},
                new Person {Name = "John", Age = 40}
            }.AsQueryable();

            var predicate = PredicateBuilder.True<Person>();
            predicate = predicate.And(ItemFilter<Person>(TokenKind.Equals, "item", "Age", testAge));
            predicate = predicate.And(ItemFilter<Person>(TokenKind.Equals, "item", "Name", testName));

            //Act
            var filtered = names.Where(predicate).ToList();

            //Assert
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual(testName, filtered[0].Name);
            Assert.AreEqual(testAge, filtered[0].Age);
        }

        [TestMethod]
        public void Filter_List_By_Age_Greater()
        {
            //Arrange
            const int testAge = 31;
            var names = new List<Person>
            {
                new Person {Name = "Michael", Age = 31},
                new Person {Name = "John", Age = 40}
            }.AsQueryable();

            var predicate = ItemFilter<Person>(TokenKind.Igt, "item", "Age", testAge);

            //Act
            var filtered = names.Where(predicate).ToList();

            //Assert
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual(testAge, filtered[0].Age);
        }

        public static Expression<Func<TItem, bool>> ItemFilter<TItem>(TokenKind op, string parameterName, string propertyName, object value) where TItem : class
        {
            var item = Expression.Parameter(typeof(TItem), parameterName);
            var prop = Expression.Property(item, propertyName);
            var compare = Expression.Constant(value);
            BinaryExpression comparison = null;
            switch (op)
            {
                case TokenKind.Ieq:
                case TokenKind.Equals:
                    comparison = Expression.Equal(prop, compare);
                    break;
                case TokenKind.Igt:
                    comparison = Expression.GreaterThan(prop, compare);
                    break;
                default:
                    return Expression.Lambda<Func<TItem, bool>>(Expression.IsTrue(Expression.Constant(true)), true);
            }

            return Expression.Lambda<Func<TItem, bool>>(comparison, item);
        }
    }
}