using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.HashingService;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Services
{
    [ModelBinder(typeof(HashedIdModelBinder))]
    public sealed class HashedId
    {
        public long Id { get; }
        public string Hashed { get; }

        public static HashedId Create(int id, IHashingService hashing)
            => new HashedId(id, hashing.HashValue(id));

        public static HashedId Create(string hashed, IHashingService hashing)
        {
            return TryCreate(hashed, hashing, out var hashedId)
                ? hashedId
                : throw new InvalidHashedIdException(hashed);
        }

        public static bool TryCreate(
            string hashed, IHashingService hashing,
            [MaybeNullWhen(false)] out HashedId hashedId)
        {
            if (hashing.TryDecodeValue(hashed, out var id))
            {
                hashedId = new HashedId(id, hashed);
                return true;
            }
            else
            {
                hashedId = default;
                return false;
            }
        }

        private HashedId(long id, string hashed)
        {
            Id = id;
            Hashed = hashed;
        }

        public override bool Equals(object? obj) => obj switch
        {
            HashedId other => other.Hashed == Hashed,
            string other => other == Hashed,
            _ => false,
        };

        public override int GetHashCode() => Hashed.GetHashCode();

        public override string ToString() => Hashed;

        public static bool operator ==(HashedId left, HashedId right) => left.Equals(right);

        public static bool operator !=(HashedId left, HashedId right) => !(left == right);
    }

    public class InvalidHashedIdException : Exception
    {
        public InvalidHashedIdException()
        {
        }

        public InvalidHashedIdException(string? hashValue)
            : base($"Invalid hashed ID value '{hashValue}'")
        {
        }

        public InvalidHashedIdException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    internal sealed class HashedIdModelBinder : IModelBinder
    {
        private readonly IHashingService customService;

        public HashedIdModelBinder(IHashingService customService)
            => this.customService = customService;

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            _ = bindingContext ?? throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var valueProvider = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProvider == ValueProviderResult.None) return Task.CompletedTask;

            var value = valueProvider.FirstValue;
            if (HashedId.TryCreate(value, customService, out var result))
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