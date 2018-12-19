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

        public static string InsertParameter(string entityName, DateTime since, DateTime until, decimal amount) =>
            string.Format(
                SepsMessages.InsertParameter,
                nameof(entityName).Humanize(LetterCasing.LowerCase),
                since,
                until,
                amount);

        public static string InsertTariff(
            string entityName, DateTime since, DateTime until, decimal lowerRate, decimal higherRate) =>
            string.Format(
                SepsMessages.InsertTariff,
                nameof(entityName).Humanize(LetterCasing.LowerCase),
                since,
                until,
                lowerRate,
                higherRate);

        public static string ParameterCorrection(string entityName, DateTime since, DateTime until, decimal amount) =>
            string.Format(
                SepsMessages.ParameterCorrection,
                nameof(entityName).Humanize(LetterCasing.LowerCase),
                since,
                until,
                amount);

        public static string SuccessfulSave() =>
            SepsMessages.SuccessfulSave;

        public static string TariffCorrection(
            string entityName, DateTime since, DateTime until, decimal lowerRate, decimal higherRate) =>
            string.Format(
                SepsMessages.TariffCorrection,
                nameof(entityName).Humanize(LetterCasing.LowerCase),
                since,
                until,
                lowerRate,
                higherRate);

        public static string ValueHigherThanTheOther(string higherEntityName, string lowerEntityName) =>
            string.Format(
                SepsMessages.ValueGreaterThanTheOther,
                nameof(higherEntityName).Humanize(LetterCasing.Sentence),
                nameof(lowerEntityName).Humanize(LetterCasing.Sentence));

        public static string ValueZeroOrAbove(string entityName) =>
            string.Format(SepsMessages.ValueZeroOrAbove, nameof(entityName).Humanize(LetterCasing.Sentence));
    }
}