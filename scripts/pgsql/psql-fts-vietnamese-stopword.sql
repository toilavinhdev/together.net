CREATE TEXT SEARCH DICTIONARY public.vietnamese (
   TEMPLATE = pg_catalog.simple,
   STOPWORDS = vietnamese
);

CREATE TEXT SEARCH CONFIGURATION public.vietnamese (
   COPY = pg_catalog.english
);

ALTER TEXT SEARCH CONFIGURATION public.vietnamese
   ALTER MAPPING
      FOR asciiword, asciihword, hword_asciipart, hword, hword_part, word
      WITH vietnamese;