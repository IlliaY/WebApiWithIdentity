using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WebApi.BLL.Exceptions;

namespace WebApi.PL.Configurations
{
    public static class ProblemDetailsConfiguration
    {
        public static void MapFluentValidationException(this ProblemDetailsOptions options) =>
            options.Map<ValidationException>((ctx, ex) =>
            {
                var factory = ctx.RequestServices.GetRequiredService<ProblemDetailsFactory>();

                var errors = ex.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(x => x.ErrorMessage).ToArray());

                return factory.CreateValidationProblemDetails(ctx, errors);
            });

        public static void MapAuthentificationException(this ProblemDetailsOptions options) =>
            options.Map<AuthentificationException>((ctx, ex) =>
            {
                var factory = ctx.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                return factory.CreateProblemDetails(ctx, ctx.Response.StatusCode, ex.Message);
            });
    }

}