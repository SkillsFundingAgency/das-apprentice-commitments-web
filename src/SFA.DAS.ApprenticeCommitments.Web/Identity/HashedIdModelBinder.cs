using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;
using SFA.DAS.Encoding;

namespace SFA.DAS.ApprenticeCommitments.Web.Identity
{
    internal sealed class HashedIdModelBinder : IModelBinder
    {
        private readonly IEncodingService _hasher;

        public HashedIdModelBinder(IEncodingService hasher)
            => _hasher = hasher;

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            _ = bindingContext ?? throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var valueProvider = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProvider == ValueProviderResult.None) return Task.CompletedTask;

            var value = valueProvider.FirstValue;
            if (HashedId.TryCreate(value, _hasher, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }
            else
            {
                bindingContext.ModelState.AddModelError(modelName, "Invalid hashed ID");
                return Task.CompletedTask;
            }
        }
    }
}