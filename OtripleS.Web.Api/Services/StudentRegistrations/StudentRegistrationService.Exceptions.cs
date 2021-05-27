﻿using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using OtripleS.Web.Api.Models.StudentRegistrations;
using OtripleS.Web.Api.Models.StudentRegistrations.Exceptions;

namespace OtripleS.Web.Api.Services.StudentRegistrations
{
    public partial class StudentRegistrationService
    {
        private delegate ValueTask<StudentRegistration> ReturningStudentRegistrationFunction();

        private async ValueTask<StudentRegistration> TryCatch(
            ReturningStudentRegistrationFunction returningStudentRegistrationFunction)
        {
            try
            {
                return await returningStudentRegistrationFunction();
            }
            catch (SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
        }

        private StudentRegistrationValidationException CreateAndLogValidationException(Exception exception)
        {
            var studentRegistrationValidationException = new StudentRegistrationValidationException(exception);
            this.loggingBroker.LogError(studentRegistrationValidationException);

            return studentRegistrationValidationException;
        }

        private StudentRegistrationDependencyException CreateAndLogCriticalDependencyException(Exception exception)
        {
            var studentRegistrationDependencyException = new StudentRegistrationDependencyException(exception);
            this.loggingBroker.LogCritical(studentRegistrationDependencyException);

            return studentRegistrationDependencyException;
        }

        private StudentRegistrationDependencyException CreateAndLogDependencyException(Exception exception)
        {
            var studentRegistrationDependencyException = new StudentRegistrationDependencyException(exception);
            this.loggingBroker.LogError(studentRegistrationDependencyException);

            return studentRegistrationDependencyException;
        }

        private StudentRegistrationServiceException CreateAndLogServiceException(Exception exception)
        {
            var StudentRegistrationServiceException = new StudentRegistrationServiceException(exception);
            this.loggingBroker.LogError(StudentRegistrationServiceException);

            return StudentRegistrationServiceException;
        }
    }
}
