--
-- PostgreSQL database dump
--

-- Dumped from database version 15.2
-- Dumped by pg_dump version 15.2

-- Started on 2024-01-21 11:32:49

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 4 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: pg_database_owner
--

CREATE SCHEMA IF NOT EXISTS public;


ALTER SCHEMA public OWNER TO pg_database_owner;

--
-- TOC entry 3470 (class 0 OID 0)
-- Dependencies: 4
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA public IS 'standard public schema';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 215 (class 1259 OID 48679)
-- Name: Assets; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Assets" (
    "Id" bigint NOT NULL,
    "Symbol" text,
    "CreatedDate" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "ProductType" bigint
);


ALTER TABLE public."Assets" OWNER TO postgres;

ALTER TABLE public."Assets" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Assets_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


CREATE TABLE public."CoinPaprikaAssets" (
    "Id" bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ),
    "Name" text,
    "Symbol" text,
    "CreatedDate" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "ProductType" bigint,
    "Provider" bigint NOT NULL
);


ALTER TABLE public."CoinPaprikaAssets" OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 48685)
-- Name: Assets_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--




--
-- TOC entry 217 (class 1259 OID 48686)
-- Name: ProviderCandlestickSyncInfos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ProviderCandlestickSyncInfos" (
    "Id" bigint NOT NULL,
    "ProviderId" bigint NOT NULL,
    "TimeframeId" bigint NOT NULL,
    "LastCandlestickSync" date
);


ALTER TABLE public."ProviderCandlestickSyncInfos" OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 48689)
-- Name: CandlestickSyncInfo_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."ProviderCandlestickSyncInfos" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."CandlestickSyncInfo_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 219 (class 1259 OID 48690)
-- Name: Candlesticks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Candlesticks" (
    "Id" bigint NOT NULL,
    pair_id bigint NOT NULL,
    timeframe integer NOT NULL,
    open_date timestamp without time zone NOT NULL,
    close_date timestamp without time zone NOT NULL,
    open_price numeric,
    high_price numeric,
    low_price numeric,
    close_price numeric,
    volume numeric,
    number_of_trades integer
);


ALTER TABLE public."Candlesticks" OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 48695)
-- Name: Candlesticks_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Candlesticks" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Candlesticks_id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 233 (class 1259 OID 48806)
-- Name: CryptoFearAndGreedIndex; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CryptoFearAndGreedIndex" (
    "PrimaryId" bigint NOT NULL,
    "Value" text NOT NULL,
    "ValueClassificationType" bigint NOT NULL,
    "DateTime" timestamp without time zone NOT NULL
);

ALTER TABLE public."CryptoFearAndGreedIndex" OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 48815)
-- Name: CryptoFearAndGreedIndex_PrimaryId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."CryptoFearAndGreedIndex" ALTER COLUMN "PrimaryId" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."CryptoFearAndGreedIndex_PrimaryId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);



CREATE TABLE public."StockFearAndGreedIndex" (
    "PrimaryId" bigint NOT NULL,
    "Value" text NOT NULL,
    "ValueClassificationType" bigint NOT NULL,
    "DateTime" timestamp without time zone NOT NULL
);


ALTER TABLE public."StockFearAndGreedIndex" OWNER TO postgres;

ALTER TABLE public."StockFearAndGreedIndex" ALTER COLUMN "PrimaryId" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."StockFearAndGreedIndex_PrimaryId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);



ALTER TABLE ONLY public."StockFearAndGreedIndex"
    ADD CONSTRAINT "unique_stockfearandgreedindex_datetime" UNIQUE ("DateTime");


--
-- TOC entry 221 (class 1259 OID 48696)
-- Name: DexCandlesticks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."DexCandlesticks" (
    "Id" bigint NOT NULL,
    "PoolContract" text,
    "PoolId" bigint,
    "OpenDate" timestamp without time zone,
    "Open" numeric,
    "High" numeric,
    "Low" numeric,
    "Close" numeric,
    "Timeframe" integer,
    "Fees" numeric,
    "Liquidity" bigint,
    "TotalValueLocked" numeric,
    "Volume" numeric,
    "TxCount" bigint
);


ALTER TABLE public."DexCandlesticks" OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 48701)
-- Name: DexCandlesticks_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."DexCandlesticks" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."DexCandlesticks_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 223 (class 1259 OID 48702)
-- Name: Pairs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Pairs" (
    id bigint NOT NULL,
    asset0_id bigint NOT NULL,
    asset1_id bigint,
    provider_id integer NOT NULL,
    symbol text NOT NULL,
    is_active boolean,
    all_candles boolean,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public."Pairs" OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 48708)
-- Name: Pairs_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Pairs" ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Pairs_id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 225 (class 1259 OID 48709)
-- Name: Pools; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Pools" (
    "Id" bigint NOT NULL,
    "DexId" bigint NOT NULL,
    "PoolContract" text NOT NULL,
    "Token0Id" bigint NOT NULL,
    "Token0Contract" text NOT NULL,
    "Token1Id" bigint NOT NULL,
    "Token1Contract" text NOT NULL,
    "FeeTier" text,
    "Fees" numeric,
    "Liquidity" bigint,
    "TotalValueLocked" numeric,
    "Volume" numeric,
    "TxCount" bigint,
    "CreatedAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "IsActive" boolean
);


ALTER TABLE public."Pools" OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 48715)
-- Name: Pools_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Pools" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Pools_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 227 (class 1259 OID 48716)
-- Name: ProviderPairAssetSyncInfos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ProviderPairAssetSyncInfos" (
    "Id" bigint NOT NULL,
    "LastAssetSync" date,
    "LastPairSync" date,
    "ProviderId" bigint
);


ALTER TABLE public."ProviderPairAssetSyncInfos" OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 48719)
-- Name: Providers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Providers" (
    "Id" bigint NOT NULL,
    "Name" text NOT NULL
);


ALTER TABLE public."Providers" OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 48724)
-- Name: Providers_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."ProviderPairAssetSyncInfos" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Providers_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 230 (class 1259 OID 48725)
-- Name: Providers_Id_seq1; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Providers" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Providers_Id_seq1"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 231 (class 1259 OID 48726)
-- Name: Timeframes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Timeframes" (
    "Id" bigint NOT NULL,
    "Name" text
);


ALTER TABLE public."Timeframes" OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 48731)
-- Name: Timeframes_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Timeframes" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Timeframes_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);

--
-- TOC entry 3458 (class 0 OID 48719)
-- Dependencies: 228
-- Data for Name: Providers; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."Providers" ("Id", "Name") VALUES
  (1, 'Binance'),
  (2, 'Uniswap'),
  (3, 'Pancakeswap'),
  (4, 'Alpaca'),
  (5, 'WallStreetZen'),
  (6, 'All'),
  (7, 'AlternativeMeCryptoFearAndGreedIndex'),
  (8, 'CoinPaprika'),
  (9, 'CoinMarketCap'),
  (10, 'CoinRanking'),
  (11, 'RapidApiCryptoFearAndGreedIndex'),
  (12, 'CnnApiCryptoFearAndGreedIndex');


--
-- TOC entry 3461 (class 0 OID 48726)
-- Dependencies: 231
-- Data for Name: Timeframes; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."Timeframes" ("Id", "Name") VALUES
  (1, 'Daily'),
  (2, 'Weekly'),
  (3, 'OneHour');


--
-- TOC entry 3471 (class 0 OID 0)
-- Dependencies: 216
-- Name: Assets_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Assets_Id_seq"', 934, true);


--
-- TOC entry 3472 (class 0 OID 0)
-- Dependencies: 218
-- Name: CandlestickSyncInfo_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."CandlestickSyncInfo_Id_seq"', 142, true);


--
-- TOC entry 3473 (class 0 OID 0)
-- Dependencies: 220
-- Name: Candlesticks_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Candlesticks_id_seq"', 525040, true);


--
-- TOC entry 3474 (class 0 OID 0)
-- Dependencies: 234
-- Name: CryptoFearAndGreedIndex_PrimaryId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."CryptoFearAndGreedIndex_PrimaryId_seq"', 7185, true);


--
-- TOC entry 3475 (class 0 OID 0)
-- Dependencies: 222
-- Name: DexCandlesticks_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."DexCandlesticks_Id_seq"', 1320, true);


--
-- TOC entry 3476 (class 0 OID 0)
-- Dependencies: 224
-- Name: Pairs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Pairs_id_seq"', 451, true);


--
-- TOC entry 3477 (class 0 OID 0)
-- Dependencies: 226
-- Name: Pools_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Pools_Id_seq"', 146, true);


--
-- TOC entry 3478 (class 0 OID 0)
-- Dependencies: 229
-- Name: Providers_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Providers_Id_seq"', 144, true);


--
-- TOC entry 3479 (class 0 OID 0)
-- Dependencies: 230
-- Name: Providers_Id_seq1; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Providers_Id_seq1"', 4, true);


--
-- TOC entry 3480 (class 0 OID 0)
-- Dependencies: 232
-- Name: Timeframes_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Timeframes_Id_seq"', 1, false);


--
-- TOC entry 3268 (class 2606 OID 48733)
-- Name: Assets Assets_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Assets"
    ADD CONSTRAINT "Assets_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3278 (class 2606 OID 48735)
-- Name: Candlesticks Candlesticks_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Candlesticks"
    ADD CONSTRAINT "Candlesticks_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3296 (class 2606 OID 48812)
-- Name: CryptoFearAndGreedIndex CryptoGreed_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CryptoFearAndGreedIndex"
    ADD CONSTRAINT "CryptoGreed_pkey" PRIMARY KEY ("PrimaryId");


--
-- TOC entry 3280 (class 2606 OID 48737)
-- Name: DexCandlesticks DexCandlesticks_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DexCandlesticks"
    ADD CONSTRAINT "DexCandlesticks_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3282 (class 2606 OID 48739)
-- Name: Pairs Pairs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pairs"
    ADD CONSTRAINT "Pairs_pkey" PRIMARY KEY (id);


--
-- TOC entry 3284 (class 2606 OID 48741)
-- Name: Pools Pools_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pools"
    ADD CONSTRAINT "Pools_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3272 (class 2606 OID 48743)
-- Name: ProviderCandlestickSyncInfos ProviderCandlestickSyncInfos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderCandlestickSyncInfos"
    ADD CONSTRAINT "ProviderCandlestickSyncInfos_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3286 (class 2606 OID 48745)
-- Name: ProviderPairAssetSyncInfos ProviderPairAssetSyncInfos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderPairAssetSyncInfos"
    ADD CONSTRAINT "ProviderPairAssetSyncInfos_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3290 (class 2606 OID 48747)
-- Name: Providers Providers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Providers"
    ADD CONSTRAINT "Providers_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3294 (class 2606 OID 48749)
-- Name: Timeframes Timeframes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Timeframes"
    ADD CONSTRAINT "Timeframes_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 3270 (class 2606 OID 48751)
-- Name: Assets unique_asset_symbol; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Assets"
    ADD CONSTRAINT unique_asset_symbol UNIQUE ("Symbol");


--
-- TOC entry 3274 (class 2606 OID 48753)
-- Name: ProviderCandlestickSyncInfos unique_providerCandlestickSyncInfos_providerId; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderCandlestickSyncInfos"
    ADD CONSTRAINT "unique_providerCandlestickSyncInfos_providerId" UNIQUE ("ProviderId");


--
-- TOC entry 3288 (class 2606 OID 48755)
-- Name: ProviderPairAssetSyncInfos unique_providerPairAssetSyncInfos_providerId; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderPairAssetSyncInfos"
    ADD CONSTRAINT "unique_providerPairAssetSyncInfos_providerId" UNIQUE ("ProviderId");


--
-- TOC entry 3276 (class 2606 OID 48774)
-- Name: ProviderCandlestickSyncInfos unique_provider_timeframe; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderCandlestickSyncInfos"
    ADD CONSTRAINT unique_provider_timeframe UNIQUE ("ProviderId", "TimeframeId");


--
-- TOC entry 3292 (class 2606 OID 48757)
-- Name: Providers unique_providers_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Providers"
    ADD CONSTRAINT unique_providers_name UNIQUE ("Name");


--
-- TOC entry 3298 (class 2606 OID 48814)
-- Name: CryptoFearAndGreedIndex unique_timestamp; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CryptoFearAndGreedIndex"
    ADD CONSTRAINT unique_timestamp UNIQUE ("DateTime");


--
-- TOC entry 3300 (class 2606 OID 48758)
-- Name: Pairs fk_assets_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pairs"
    ADD CONSTRAINT fk_assets_id FOREIGN KEY (asset0_id) REFERENCES public."Assets"("Id") NOT VALID;


--
-- TOC entry 3299 (class 2606 OID 48768)
-- Name: ProviderCandlestickSyncInfos fk_provider_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderCandlestickSyncInfos"
    ADD CONSTRAINT fk_provider_id FOREIGN KEY ("ProviderId") REFERENCES public."Providers"("Id") NOT VALID;


--
-- TOC entry 3302 (class 2606 OID 48816)
-- Name: ProviderPairAssetSyncInfos fk_provider_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderPairAssetSyncInfos"
    ADD CONSTRAINT fk_provider_id FOREIGN KEY ("ProviderId") REFERENCES public."Providers"("Id") NOT VALID;


-- Completed on 2024-01-21 11:32:49

--
-- PostgreSQL database dump complete
--

