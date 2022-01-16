# SELibDotNet

A .NET library for reading and writing SE Format files

*.SE formats are open-sourced formats optimized for next-generation modeling and animation. They are free to be used in any project, game, software, etc with the hopes that people will adapt the standard unlike other formats available.*

- Animation format documentation: [Specification](https://github.com/SE2Dev/SEAnim-Docs)
- Model format documentation: [Coming soon](#)

## Usage:

- Download the latest version, add it as a reference to your .NET based project and start working with SE Formats
- **Note:** SEAnim format is unitless for translations, a scale of 1.0 = default scaling, and rotations are XYZW. Please be sure to encode the input properly as there is no way for the library to determine otherwise!
- (Check out UnitTests for examples on using the library, everything has full documentation though.)
