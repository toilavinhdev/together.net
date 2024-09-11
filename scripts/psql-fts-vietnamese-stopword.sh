#!/bin/sh

# shellcheck disable=SC2046
echo $(pg_config --sharedir)/tsearch_data &&
cd $(pg_config --sharedir)/tsearch_data || exit &&
wget https://raw.githubusercontent.com/stopwords/vietnamese-stopwords/master/vietnamese-stopwords.txt &&
mv vietnamese-stopwords.txt vietnamese.stop &&
sudo systemctl reload postgresql