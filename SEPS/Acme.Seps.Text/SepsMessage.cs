using Humanizer;
using System;

namespace Acme.Seps.Text
{
    public static class SepsMessage
    {
        public static string CannotDeactivateInactiveEntity(string entityName) =>
            string.Format(
                SepsMessages.CannotDeactivateInactiveEntity, nameof(entityName).Humanize(LetterCasing.Sentence));

        public static string EntityNotSet(string entityName) =>
            string.Format(SepsMessages.EntityNotSet, nameof(entityName).Humanize(LetterCasing.Sentence));

        public static string InactiveException(string entityName) =>
            string.Format(SepsMessages.InactiveException, nameof(entityName).Humanize(LetterCasing.Sentence));

        public static string InitialValuesMustNotBeChanged() =>
            SepsMessages.InitialValuesMustNotBeChanged;

        public static string InsertParameter(string entityName, DateTimeOffset since, DateTimeOffset? until, decimal amount) =>
            string.Format(
                SepsMessages.InsertParameter,
                nameof(entityName).Humanize(LetterCasing.LowerCase),
                since,
                GetUntil(until),
                amount);

        public static string InsertTariff(
            string entityName, DateTimeOffset since, DateTimeOffset? until, decimal lowerRate, decimal higherRate) =>
            string.Format(
                SepsMessages.InsertTariff,
                nameof(entityName).Humanize(LetterCasing.LowerCase),
                since,
                GetUntil(until),
                lowerRate,
                higherRate);

        public static string ParameterCorrection(string entityName, DateTimeOffset since, DateTimeOffset? until, decimal amount) =>
            string.Format(
                SepsMessages.ParameterCorrection,
                nameof(entityName).Humanize(LetterCasing.LowerCase),
                since,
                GetUntil(until),
                amount);

        public static string SuccessfulSave() =>
            SepsMessages.SuccessfulSave;

        public static string TariffCorrection(
            string entityName, DateTimeOffset since, DateTimeOffset? until, decimal lowerRate, decimal higherRate) =>
            string.Format(
                SepsMessages.TariffCorrection,
                nameof(entityName).Humanize(LetterCasing.LowerCase),
                since,
                GetUntil(until),
                lowerRate,
                higherRate);

        public static string ValueHigherThanTheOther(string higherEntityName, string lowerEntityName) =>
            string.Format(
                SepsMessages.ValueGreaterThanTheOther,
                nameof(higherEntityName).Humanize(LetterCasing.Sentence),
                nameof(lowerEntityName).Humanize(LetterCasing.Sentence));

        public static string ValueZeroOrAbove(string entityName) =>
            string.Format(SepsMessages.ValueZeroOrAbove, nameof(entityName).Humanize(LetterCasing.Sentence));

        private static string GetUntil(DateTimeOffset? until) =>
            until.HasValue ? until.Value.Date.ToShortDateString() : SepsMessages.Undefined;
    }
}