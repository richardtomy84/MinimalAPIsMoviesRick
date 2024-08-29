namespace MinimalAPIsMoviesRick.Validaton
{
    public static class ValidationUtilities
    {
        public static string NonEmptyMessage = "The field {PropertyName} is required";
        public static string MaximunLengthMessage = "The field {PropertyName} should be less than {MaxLength} character";
        public static string FirstLetterIsUpperCaseMessage = "The field {PropertyName} should start with upercase";
        public static string GreaterThanDate(DateTime value) => "The field {PropertyName} should be greater than " + value.ToString("yyyy-MM-dd")
        public static  bool FirstLetterIsUppercase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }

    }
}
  