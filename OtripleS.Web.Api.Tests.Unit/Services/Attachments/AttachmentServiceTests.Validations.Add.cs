﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Moq;
using OtripleS.Web.Api.Models.Attachments;
using OtripleS.Web.Api.Models.Attachments.Exceptions;
using Xunit;

namespace OtripleS.Web.Api.Tests.Unit.Services.Attachments
{
    public partial class AttachmentServiceTests
    {
        [Fact]
        public async void ShouldThrowValidationExceptionOnCreateWhenAttachmentIsNullAndLogItAsync()
        {
            // given
            Attachment randomAttachment = null;
            Attachment nullAttachment = randomAttachment;
            var nullAttachmentException = new NullAttachmentException();

            var expectedAttachmentValidationException =
                new AttachmentValidationException(nullAttachmentException);

            // when
            ValueTask<Attachment> createAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(nullAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                createAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttachmentByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnCreateWhenIdIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Attachment randomAttachment = CreateRandomAttachment(dateTime);
            Attachment inputAttachment = randomAttachment;
            inputAttachment.Id = default;

            var invalidAttachmentInputException = new InvalidAttachmentException(
                parameterName: nameof(Attachment.Id),
                parameterValue: inputAttachment.Id);

            var expectedAttachmentValidationException =
                new AttachmentValidationException(invalidAttachmentInputException);

            // when
            ValueTask<Attachment> registerAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(inputAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                registerAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttachmentByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnCreateWhenAttachmentLabelIsInvalidAndLogItAsync(
            string invalidAttachmentLabel)
        {
            // given
            Attachment randomAttachment = CreateRandomAttachment();
            Attachment invalidAttachment = randomAttachment;
            invalidAttachment.Label = invalidAttachmentLabel;

            var invalidAttachmentException = new InvalidAttachmentException(
               parameterName: nameof(Attachment.Label),
               parameterValue: invalidAttachment.Label);

            var expectedAttachmentValidationException =
                new AttachmentValidationException(invalidAttachmentException);

            // when
            ValueTask<Attachment> createAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(invalidAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                createAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnCreateWhenAttachmentDescriptionIsInvalidAndLogItAsync(
            string invalidAttachmentDescription)
        {
            // given
            Attachment randomAttachment = CreateRandomAttachment();
            Attachment invalidAttachment = randomAttachment;
            invalidAttachment.Description = invalidAttachmentDescription;

            var invalidAttachmentException = new InvalidAttachmentException(
               parameterName: nameof(Attachment.Description),
               parameterValue: invalidAttachment.Description);

            var expectedAttachmentValidationException =
                new AttachmentValidationException(invalidAttachmentException);

            // when
            ValueTask<Attachment> createAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(invalidAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                createAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnCreateWhenAttachmentContectTypeIsInvalidAndLogItAsync(
            string invalidAttachmentContectType)
        {
            // given
            Attachment randomAttachment = CreateRandomAttachment();
            Attachment invalidAttachment = randomAttachment;
            invalidAttachment.ContectType = invalidAttachmentContectType;

            var invalidAttachmentException = new InvalidAttachmentException(
               parameterName: nameof(Attachment.ContectType),
               parameterValue: invalidAttachment.ContectType);

            var expectedAttachmentValidationException =
                new AttachmentValidationException(invalidAttachmentException);

            // when
            ValueTask<Attachment> createAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(invalidAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                createAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnCreateWhenAttachmentExtensionIsInvalidAndLogItAsync(
            string invalidAttachmentExtension)
        {
            // given
            Attachment randomAttachment = CreateRandomAttachment();
            Attachment invalidAttachment = randomAttachment;
            invalidAttachment.Extension = invalidAttachmentExtension;

            var invalidAttachmentException = new InvalidAttachmentException(
               parameterName: nameof(Attachment.Extension),
               parameterValue: invalidAttachment.Extension);

            var expectedAttachmentValidationException =
                new AttachmentValidationException(invalidAttachmentException);

            // when
            ValueTask<Attachment> createAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(invalidAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                createAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnCreateWhenCreatedByIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Attachment randomAttachment = CreateRandomAttachment(dateTime);
            Attachment inputAttachment = randomAttachment;
            inputAttachment.CreatedBy = default;

            var invalidAttachmentInputException = new InvalidAttachmentException(
                parameterName: nameof(Attachment.CreatedBy),
                parameterValue: inputAttachment.CreatedBy);

            var expectedAttachmentValidationException =
                new AttachmentValidationException(invalidAttachmentInputException);

            // when
            ValueTask<Attachment> createAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(inputAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                createAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttachmentByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnCreateWhenCreatedDateIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Attachment randomAttachment = CreateRandomAttachment(dateTime);
            Attachment inputAttachment = randomAttachment;
            inputAttachment.CreatedDate = default;

            var invalidAttachmentInputException = new InvalidAttachmentException(
                parameterName: nameof(Attachment.CreatedDate),
                parameterValue: inputAttachment.CreatedDate);

            var expectedAttachmentValidationException =
                new AttachmentValidationException(invalidAttachmentInputException);

            // when
            ValueTask<Attachment> createAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(inputAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                createAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttachmentByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnCreateWhenUpdatedByIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Attachment randomAttachment = CreateRandomAttachment(dateTime);
            Attachment inputAttachment = randomAttachment;
            inputAttachment.UpdatedBy = default;

            var invalidAttachmentInputException = new InvalidAttachmentException(
                parameterName: nameof(Attachment.UpdatedBy),
                parameterValue: inputAttachment.UpdatedBy);

            var expectedAttachmentValidationException =
                new AttachmentValidationException(invalidAttachmentInputException);

            // when
            ValueTask<Attachment> createAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(inputAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                createAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttachmentByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnCreateWhenUpdatedDateIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Attachment randomAttachment = CreateRandomAttachment(dateTime);
            Attachment inputAttachment = randomAttachment;
            inputAttachment.UpdatedDate = default;

            var invalidAttachmentInputException = new InvalidAttachmentException(
                parameterName: nameof(Attachment.UpdatedDate),
                parameterValue: inputAttachment.UpdatedDate);

            var expectedAttachmentValidationException =
                new AttachmentValidationException(invalidAttachmentInputException);

            // when
            ValueTask<Attachment> createAttachmentTask =
                this.attachmentService.InsertAttachmentAsync(inputAttachment);

            // then
            await Assert.ThrowsAsync<AttachmentValidationException>(() =>
                createAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttachmentValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttachmentByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
