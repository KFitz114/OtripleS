﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using Moq;
using OtripleS.Web.Api.Models.Students;
using OtripleS.Web.Api.Models.Students.Exceptions;
using Xunit;

namespace OtripleS.Web.Api.Tests.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyWhenStudentIsNullAndLogItAsync()
        {
            // given
            Student invalidStudent = null;
            var nullStudentException = new NullStudentException();

            var expectedStudentValidationException =
                new StudentValidationException(nullStudentException);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            // then
            await Assert.ThrowsAsync<StudentValidationException>(() =>
                modifyStudentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnModifyWhenStudentIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidStudent = new Student
            {
                UserId = invalidText,
                IdentityNumber = invalidText,
                FirstName = invalidText
            };

            var invalidStudentException = new InvalidStudentException();

            invalidStudentException.AddData(
                key: nameof(Student.Id),
                values: "Id is required");

            invalidStudentException.AddData(
                key: nameof(Student.UserId),
                values: "Text is required");

            invalidStudentException.AddData(
                key: nameof(Student.IdentityNumber),
                values: "Text is required");

            invalidStudentException.AddData(
                key: nameof(Student.FirstName),
                values: "Text is required");

            invalidStudentException.AddData(
                key: nameof(Student.BirthDate),
                values: "Date is required");

            invalidStudentException.AddData(
                key: nameof(Student.CreatedBy),
                values: "Id is required");

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedBy),
                values: "Id is required");

            invalidStudentException.AddData(
                key: nameof(Student.CreatedDate),
                values: "Date is required");

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values: "Date is required");

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            // then
            await Assert.ThrowsAsync<StudentValidationException>(() =>
                modifyStudentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetCurrentDateTime(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogError(It.Is(SameValidationExceptionAs(
                        expectedStudentValidationException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.UpdateStudentAsync(It.IsAny<Student>()),
                Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAndUpdatedDatesAreSameAndLogItAsync()
        {
            // given
            Student randomStudent = CreateRandomStudent();
            DateTimeOffset sameDate = randomStudent.CreatedDate;
            Student invalidStudent = randomStudent;
            invalidStudent.CreatedDate = sameDate;
            invalidStudent.UpdatedDate = sameDate;

            var invalidStudentException = new InvalidStudentException();

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values: $"Date is the same as { nameof(Student.CreatedDate)}");

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            // then
            await Assert.ThrowsAsync<StudentValidationException>(() =>
                modifyStudentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinuteCases))]
        public async Task ShouldThrowValidationExceptionOnModifyIfStudentUpdatedDateIsNotRecentAndLogItAsync(
            int randomMoreOrLessThanOneMinute)
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Student randomStudent = CreateRandomStudent(dates: randomDateTime);
            Student invalidStudent = randomStudent;

            invalidStudent.UpdatedDate =
                invalidStudent.UpdatedDate.AddMinutes(randomMoreOrLessThanOneMinute);

            var invalidStudentException = new InvalidStudentException(
                parameterName: nameof(Student.UpdatedDate),
                parameterValue: invalidStudent.UpdatedDate);

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTime())
                .Returns(randomDateTime);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            // then
            await Assert.ThrowsAsync<StudentValidationException>(() =>
                modifyStudentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetCurrentDateTime(),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStudentDoesntExistAndLogItAsync()
        {
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Student randomStudent = CreateRandomStudent();
            Student nonExistentStudent = randomStudent;
            nonExistentStudent.UpdatedDate = randomDateTime;
            Student noStudent = null;
            var notFoundStudentException = new NotFoundStudentException(nonExistentStudent.Id);

            var expectedStudentValidationException =
                new StudentValidationException(notFoundStudentException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectStudentByIdAsync(nonExistentStudent.Id))
                .ReturnsAsync(noStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTime())
                .Returns(randomDateTime);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(nonExistentStudent);

            // then
            await Assert.ThrowsAsync<StudentValidationException>(() =>
                modifyStudentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetCurrentDateTime(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectStudentByIdAsync(nonExistentStudent.Id),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreateDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDate = GetRandomDateTime();
            Student randomStudent = CreateRandomStudent();
            Student invalidStudent = randomStudent;
            invalidStudent.UpdatedDate = randomDate;
            Student storageStudent = randomStudent.DeepClone();
            Guid studentId = invalidStudent.Id;
            invalidStudent.CreatedDate = storageStudent.CreatedDate.AddMinutes(randomNumber);

            var invalidStudentException = new InvalidStudentException(
                parameterName: nameof(Student.CreatedDate),
                parameterValue: invalidStudent.CreatedDate);

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectStudentByIdAsync(studentId))
                .ReturnsAsync(storageStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTime())
                .Returns(randomDate);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            // then
            await Assert.ThrowsAsync<StudentValidationException>(() =>
                modifyStudentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetCurrentDateTime(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectStudentByIdAsync(invalidStudent.Id),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedByNotSameAsCreatedByAndLogItAsync()
        {
            // given
            Guid differentId = Guid.NewGuid();
            Guid invalidCreatedBy = differentId;
            DateTimeOffset randomDate = GetRandomDateTime();
            Student randomStudent = CreateRandomStudent();
            Student invalidStudent = randomStudent;
            invalidStudent.UpdatedDate = randomDate;
            Student storageStudent = randomStudent.DeepClone();
            Guid studentId = invalidStudent.Id;
            invalidStudent.CreatedBy = invalidCreatedBy;

            var invalidStudentException = new InvalidStudentException(
                parameterName: nameof(Student.CreatedBy),
                parameterValue: invalidStudent.CreatedBy);

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectStudentByIdAsync(studentId))
                .ReturnsAsync(storageStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTime())
                .Returns(randomDate);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            // then
            await Assert.ThrowsAsync<StudentValidationException>(() =>
                modifyStudentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetCurrentDateTime(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectStudentByIdAsync(invalidStudent.Id),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomNegativeMinutes = GetNegativeRandomNumber();
            int minutesInThePast = randomNegativeMinutes;
            DateTimeOffset randomDate = GetRandomDateTime();
            Student randomStudent = CreateRandomStudent(dates: randomDate);
            randomStudent.CreatedDate = randomStudent.CreatedDate.AddMinutes(minutesInThePast);
            Student invalidStudent = randomStudent;
            invalidStudent.UpdatedDate = randomDate;
            Student storageStudent = randomStudent.DeepClone();
            Guid studentId = invalidStudent.Id;

            var invalidStudentException = new InvalidStudentException(
                parameterName: nameof(Student.UpdatedDate),
                parameterValue: invalidStudent.UpdatedDate);

            var expectedStudentValidationException =
                new StudentValidationException(invalidStudentException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectStudentByIdAsync(studentId))
                .ReturnsAsync(storageStudent);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTime())
                .Returns(randomDate);

            // when
            ValueTask<Student> modifyStudentTask =
                this.studentService.ModifyStudentAsync(invalidStudent);

            // then
            await Assert.ThrowsAsync<StudentValidationException>(() =>
                modifyStudentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetCurrentDateTime(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                    broker.SelectStudentByIdAsync(invalidStudent.Id),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                    broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))),
                Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}