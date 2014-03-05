package weather;


public enum WeatherType {

    RAIN {
        @Override
        public String toString() {
            return "It's raining!";
        }
    },

    CLOUDY {
        @Override
        public String toString() {
            return "It's overcast, tons of clouds all around!";
        }
    },

    SUNNY {
        @Override
        public String toString() {
            return "Bright sunlight, 40 degrees and burning!";
        }
    },

    CLOUDYWITHACHANCEOFMEATBALLS {
        @Override
        public String toString() {
            return "MASSIVE MEATBALLS, HACK ATTACK ;-)";
        }
    }

}
