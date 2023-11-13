--
-- PostgreSQL database dump
--

-- Dumped from database version 15.2
-- Dumped by pg_dump version 15.2

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
-- Name: public; Type: SCHEMA; Schema: -; Owner: pg_database_owner
--

DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.schemata WHERE schema_name = 'public') THEN
        CREATE SCHEMA public;
    END IF;
END $$;


ALTER SCHEMA public OWNER TO pg_database_owner;

--
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA public IS 'standard public schema';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Assets; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Assets" (
    "Id" bigint NOT NULL,
    "Symbol" text,
    "CreatedDate" timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public."Assets" OWNER TO postgres;

--
-- Name: Assets_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Assets" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Assets_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ProviderCandlestickSyncInfos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ProviderCandlestickSyncInfos" (
    "Id" bigint NOT NULL,
    "ProviderId" bigint NOT NULL,
    "TimeframeId" bigint NOT NULL,
    "LastCandlestickSync" date
);


ALTER TABLE public."ProviderCandlestickSyncInfos" OWNER TO postgres;

ALTER TABLE public."ProviderCandlestickSyncInfos"
ADD CONSTRAINT provider_timeframe_unique UNIQUE ("ProviderId", "TimeframeId");

--
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
-- Name: Providers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Providers" (
    "Id" bigint NOT NULL,
    "Name" text NOT NULL
);


ALTER TABLE public."Providers" OWNER TO postgres;

--
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
-- Name: Timeframes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Timeframes" (
    "Id" bigint NOT NULL,
    "Name" text
);


ALTER TABLE public."Timeframes" OWNER TO postgres;

--
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
-- Data for Name: Providers; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."Providers" ("Id", "Name") VALUES
  (1, 'Binance'),
  (2, 'Uniswap'),
  (3, 'Pancakeswap'),
  (4, 'Alpaca'),
  (5, 'WallStreetZen'),
  (6, 'All');


--
-- Data for Name: Timeframes; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."Timeframes" ("Id", "Name") VALUES
  (1, 'Daily'),
  (2, 'Weekly'),
  (3, 'OneHour');


--
-- Name: Assets_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Assets_Id_seq"', 3, true);


--
-- Name: CandlestickSyncInfo_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."CandlestickSyncInfo_Id_seq"', 1, false);


--
-- Name: Candlesticks_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Candlesticks_id_seq"', 1, false);


--
-- Name: DexCandlesticks_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."DexCandlesticks_Id_seq"', 1, false);


--
-- Name: Pairs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Pairs_id_seq"', 1, false);


--
-- Name: Pools_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Pools_Id_seq"', 1, false);


--
-- Name: Providers_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Providers_Id_seq"', 1, false);


--
-- Name: Providers_Id_seq1; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Providers_Id_seq1"', 4, true);


--
-- Name: Timeframes_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Timeframes_Id_seq"', 1, false);


--
-- Name: Assets Assets_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Assets"
    ADD CONSTRAINT "Assets_pkey" PRIMARY KEY ("Id");


--
-- Name: Candlesticks Candlesticks_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Candlesticks"
    ADD CONSTRAINT "Candlesticks_pkey" PRIMARY KEY ("Id");


--
-- Name: DexCandlesticks DexCandlesticks_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DexCandlesticks"
    ADD CONSTRAINT "DexCandlesticks_pkey" PRIMARY KEY ("Id");


--
-- Name: Pairs Pairs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pairs"
    ADD CONSTRAINT "Pairs_pkey" PRIMARY KEY (id);


--
-- Name: Pools Pools_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pools"
    ADD CONSTRAINT "Pools_pkey" PRIMARY KEY ("Id");


--
-- Name: ProviderCandlestickSyncInfos ProviderCandlestickSyncInfos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderCandlestickSyncInfos"
    ADD CONSTRAINT "ProviderCandlestickSyncInfos_pkey" PRIMARY KEY ("Id");


--
-- Name: ProviderPairAssetSyncInfos ProviderPairAssetSyncInfos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderPairAssetSyncInfos"
    ADD CONSTRAINT "ProviderPairAssetSyncInfos_pkey" PRIMARY KEY ("Id");


--
-- Name: Providers Providers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Providers"
    ADD CONSTRAINT "Providers_pkey" PRIMARY KEY ("Id");


--
-- Name: Timeframes Timeframes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Timeframes"
    ADD CONSTRAINT "Timeframes_pkey" PRIMARY KEY ("Id");


--
-- Name: Assets unique_asset_symbol; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Assets"
    ADD CONSTRAINT unique_asset_symbol UNIQUE ("Symbol");


--
-- Name: ProviderPairAssetSyncInfos unique_providerPairAssetSyncInfos_providerId; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderPairAssetSyncInfos"
    ADD CONSTRAINT "unique_providerPairAssetSyncInfos_providerId" UNIQUE ("ProviderId");


--
-- Name: Providers unique_providers_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Providers"
    ADD CONSTRAINT unique_providers_name UNIQUE ("Name");


--
-- Name: Pairs fk_assets_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pairs"
    ADD CONSTRAINT fk_assets_id FOREIGN KEY (asset0_id) REFERENCES public."Assets"("Id") NOT VALID;


--
-- Name: Pools fk_assets_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pools"
    ADD CONSTRAINT fk_assets_id FOREIGN KEY ("Id") REFERENCES public."Assets"("Id") NOT VALID;


--
-- Name: ProviderCandlestickSyncInfos fk_provider_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ProviderCandlestickSyncInfos"
    ADD CONSTRAINT fk_provider_id FOREIGN KEY ("ProviderId") REFERENCES public."Providers"("Id") NOT VALID;


--
-- PostgreSQL database dump complete
--

