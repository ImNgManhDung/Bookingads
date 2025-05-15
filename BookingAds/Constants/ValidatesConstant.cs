using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Constants
{
    public static class ValidatesConstant
    {
        public const string UPPER = "[A-Z]";
        public const string LOWER = "[a-z]";
        public const string DIGIT = "[0-9]";
        public const string NOT_DIGIT = "[^0-9]";
        public const string OTHER = "[^A-Za-z0-9]";
        public const string SPECIAL = "[ %&*+./:<>?\\\\]";

        public const int DFCODE = 6;
        public const int MIN_PASSWORD = 8;
        public const int MAX_PASSWORD = 20;

        public const int MAX_AREATEXTBOX = 300;
        public const int MIN_AREATEXTBOX = 1;

        public const int MIN_FOOD_NAME = 3;
        public const int MAX_FOOD_NAME = 300;

        public const int MIN_QUANTITY_FOOD = 1;
        public const int MAX_QUANTITY_FOOD = 5000;

        public const int MIN_PRICE_FOOD = 1000;
        public const long MAX_PRICE_FOOD = 1000000000;

        public const int MIN_LASTNAME = 2;
        public const int MAX_LASTNAME = 100;

        public const int MIN_FIRSTNAME = 1;
        public const int MAX_FIRSTNAME = 50;

        public const int PHONE_LENGTH = 9;
    }
}