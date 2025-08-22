using NUnit.Framework;
using System.ComponentModel;
using SFA.DAS.ApprenticeCommitments.Web.Models;
using SFA.DAS.ApprenticeCommitments.Web.Helpers;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Helpers
{
    [TestFixture]
    public class UtilitiesTests
    {
        [Test]
        public void GetApprenticeshipTypeDescription_ReturnsCorrectDescription_ForValidApprenticeship()
        {
            // Arrange
            int? input = (int)ApprenticeshipType.Apprenticeship;
            
            // Act
            var result = Utilities.GetApprenticeshipTypeDescription(input);
            
            // Assert
            Assert.AreEqual("Apprenticeship", result);
        }
        
        [Test]
        public void GetApprenticeshipTypeDescription_ReturnsNumericString_ForInvalidEnumValue()
        {
            // Arrange
            int? input = 99;
            
            // Act
            var result = Utilities.GetApprenticeshipTypeDescription(input);
            
            // Assert
            Assert.AreEqual("99", result);
        }

        [Test]
        public void GetApprenticeshipTypeDescription_ReturnsEmptyString_ForNullInput()
        {
            // Arrange
            int? input = null;
            
            // Act
            var result = Utilities.GetApprenticeshipTypeDescription(input);
            
            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void GetApprenticeshipTypeDescription_ReturnsNegativeNumberString_ForNegativeInput()
        {
            // Arrange
            int? input = -1;
            
            // Act
            var result = Utilities.GetApprenticeshipTypeDescription(input);
            
            // Assert
            Assert.AreEqual("-1", result);
        }

        [Test]
        public void IsValidApprenticeshipType_ReturnsTrue_ForValidApprenticeship()
        {
            // Arrange
            int? input = (int)ApprenticeshipType.Apprenticeship;
            
            // Act
            var result = Utilities.IsValidApprenticeshipType(input);
            
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValidApprenticeshipType_ReturnsTrue_ForValidFoundationApprenticeship()
        {
            // Arrange
            int? input = (int)ApprenticeshipType.FoundationApprenticeship;
            
            // Act
            var result = Utilities.IsValidApprenticeshipType(input);
            
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValidApprenticeshipType_ReturnsFalse_ForNullInput()
        {
            // Arrange
            int? input = null;
            
            // Act
            var result = Utilities.IsValidApprenticeshipType(input);
            
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidApprenticeshipType_ReturnsFalse_ForInvalidPositiveValue()
        {
            // Arrange
            int? input = 2;
            
            // Act
            var result = Utilities.IsValidApprenticeshipType(input);
            
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidApprenticeshipType_ReturnsFalse_ForNegativeValue()
        {
            // Arrange
            int? input = -1;
            
            // Act
            var result = Utilities.IsValidApprenticeshipType(input);
            
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidApprenticeshipType_ReturnsFalse_ForMaxIntValue()
        {
            // Arrange
            int? input = int.MaxValue;
            
            // Act
            var result = Utilities.IsValidApprenticeshipType(input);
            
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidApprenticeshipType_ReturnsFalse_ForMinIntValue()
        {
            // Arrange
            int? input = int.MinValue;
            
            // Act
            var result = Utilities.IsValidApprenticeshipType(input);
            
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void BothMethods_HandleBoundaryValues_Consistently()
        {
            // Test both methods with the same boundary value
            int? input = 0;
            
            // Assert both methods handle valid boundary
            Assert.IsTrue(Utilities.IsValidApprenticeshipType(input));
            Assert.AreEqual("Apprenticeship", Utilities.GetApprenticeshipTypeDescription(input));

            input = 1;
            Assert.IsTrue(Utilities.IsValidApprenticeshipType(input));
            Assert.AreEqual("Foundation apprenticeship", Utilities.GetApprenticeshipTypeDescription(input));

            input = 2;
            Assert.IsFalse(Utilities.IsValidApprenticeshipType(input));
            Assert.AreEqual("2", Utilities.GetApprenticeshipTypeDescription(input));
        }
    }
}