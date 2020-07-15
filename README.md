# Synapse Demo Data Generator

## About
This project came from the need to create data quickly while still having valid (looking) data. Our goal is to have several industry types worth of data easily able to be created on demand from the command line. This is essentially a wrapper around the [Bogus Library](https://github.com/bchavez/Bogus).

## Usage
Run with ``` SynapseDemoDataGenerator.exe -h ``` to get an output of all the options. In general the syntax will be ``` SynapseDemoDataGenerator.exe [DataType] [RelatedOptions] ```

### Warning
Creating more than 5,000,000 rows in a single run will utilize disk based caching to prevent memory issues. This can be slow as it will be 100% based on your disk access speeds.

## Contributing

Currently we need help modeling different industries, this can be as simple as providing us general ideas of what data is needed for one of the industries you have experience in.

* [X] ~~*Retail*~~ [Handled by Rental]
* [ ] Healthcare [Hospital?]
* [ ] Financial Services [Bank?]
* [ ] Manfacturing [Assembly Line?]

