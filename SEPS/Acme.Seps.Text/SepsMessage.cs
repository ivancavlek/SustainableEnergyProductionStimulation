using Humanizer;
using System;

namespace Acme.Seps.Text
{
    public static class SepsMessage
    {
        public static string CannotDeactivateInactiveEntity(string entityName) =>
            string.Format(
                SepsMessages.CannotDeactivateInactiveEntity, entityName.Humanize(LetterCasing.Sentence));

        public static string EntityNotSet(string entityName) =>
            string.Format(SepsMessages.EntityNotSet, entityName.Humanize(LetterCasing.Sentence));

        public static string InactiveException(string entityName) =>
            string.Format(SepsMessages.InactiveException, entityName.Humanize(LetterCasing.Sentence));

        public static string InitialValuesMustNotBeChanged() =>
            SepsMessages.InitialValuesMustNotBeChanged;

        public static string InsertParameter(string entityName, DateTimeOffset since, DateTimeOffset? until, decimal amount) =>
            string.Format(
                SepsMessages.InsertParameter,
                entityName.Humanize(LetterCasing.LowerCase),
                since,
                GetUntil(until),
                amount);

        public static string InsertTariff(
            string entityName, DateTimeOffset since, DateTimeOffset? until, decimal lowerRate, decimal higherRate) =>
            string.Format(
                SepsMessages.InsertTariff,
                entityName.Humanize(LetterCasing.LowerCase),
                since,
                GetUntil(until),
                lowerRate,
                higherRate);

        public static string ParameterCorrection(string entityName, DateTimeOffset since, DateTimeOffset? until, decimal amount) =>
            string.Format(
                SepsMessages.ParameterCorrection,
                entityName.Humanize(LetterCasing.LowerCase),
                since,
                GetUntil(until),
                amount);

        public static string SuccessfulSave() =>
            SepsMessages.SuccessfulSave;

        public static string TariffCorrection(
            string entityName, DateTimeOffset since, DateTimeOffset? until, decimal lowerRate, decimal higherRate) =>
            string.Format(
                SepsMessages.TariffCorrection,
                entityName.Humanize(LetterCasing.LowerCase),
                since,
                GetUntil(until),
                lowerRate,
                higherRate);

        public static string ValueHigherThanTheOther(string higherEntityName, string lowerEntityName) =>
            string.Format(
                SepsMessages.ValueGreaterThanTheOther,
                higherEntityName.Humanize(LetterCasing.Sentence),
                lowerEntityName.Humanize(LetterCasing.LowerCase));

        public static string ValueZeroOrAbove(string entityName) =>
            string.Format(SepsMessages.ValueZeroOrAbove, entityName.Humanize(LetterCasing.Sentence));

        private static string GetUntil(DateTimeOffset? until) =>
            until.HasValue ? until.Value.Date.ToShortDateString() : SepsMessages.Undefined;
    }
}