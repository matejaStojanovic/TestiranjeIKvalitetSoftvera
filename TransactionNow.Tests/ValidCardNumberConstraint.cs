using NUnit.Framework.Constraints;
using System.Linq;

namespace TransactionNow.Tests.CustomConstraints
{
    public class ValidCardNumberConstraint : Constraint
    {
        public override string Description =>
            "a valid card number (minimum 8 digits, numeric only)";

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is not string cardNumber)
                return new ConstraintResult(this, actual, isSuccess: false);

            bool isValid =
                cardNumber.Length >= 8 &&
                cardNumber.All(char.IsDigit);

            return new ConstraintResult(this, actual, isValid);
        }
    }
}
