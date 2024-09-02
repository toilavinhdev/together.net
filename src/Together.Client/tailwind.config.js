const defaultColors = require("tailwindcss/colors");
delete defaultColors["lightBlue"];
delete defaultColors["warmGray"];
delete defaultColors["trueGray"];
delete defaultColors["coolGray"];
delete defaultColors["blueGray"];

const customColors = {
  primary: {
    DEFAULT: "var(--primary)",
    lighter: "var(--primary-lighter)",
  },
};

/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,js}"],
  theme: {
    extend: {
      colors: {
        ...defaultColors,
        ...customColors,
      },
    },
  },
  plugins: [],
  corePlugins: {
    preflight: false,
  },
};
