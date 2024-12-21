--
-- PostgreSQL database dump
--

-- Dumped from database version 17rc1
-- Dumped by pg_dump version 17rc1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: fn_buscar_prioridade(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_buscar_prioridade() RETURNS TABLE(res_codigo integer, res_nivel character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
  RETURN QUERY SELECT codigo, nivel FROM tb_stc_nivel_prioridade;
END;
$$;


ALTER FUNCTION public.fn_buscar_prioridade() OWNER TO postgres;

--
-- Name: fn_cad_parceiro_negocio_dados(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_cad_parceiro_negocio_dados(p_codigo integer, p_codigo_empresa integer) RETURNS TABLE(codigo integer, is_cnpj boolean, documento character varying, telefone character varying, email character varying, data_cadastro timestamp without time zone, tipo_parceiro character, codigo_empresa integer, nome_fantasia character varying, razao_social character varying, contato character varying, codigo_pais bigint, codigo_cidade bigint, codigo_estado integer, nome_pais character varying, nome_cidade character varying, uf_estado character varying, bairro character varying, logradouro character varying, numero character varying, cep character varying, complemento character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        tb_cad_parceiro_negocio.codigo,
        tb_cad_parceiro_negocio.is_cnpj,
        tb_cad_parceiro_negocio.documento,
        COALESCE(tb_cad_parceiro_negocio.telefone, ''),
        COALESCE(tb_cad_parceiro_negocio.email, ''),
        tb_cad_parceiro_negocio.data_cadastro,
        tb_cad_parceiro_negocio.tipo_parceiro,
        tb_cad_parceiro_negocio.codigo_empresa,
        COALESCE(tb_cad_parceiro_negocio.nome_fantasia, ''),
        COALESCE(tb_cad_parceiro_negocio.razao_social, ''),
        COALESCE(tb_cad_parceiro_negocio.contato, ''),
        COALESCE(tb_cad_parceiro_negocio.codigo_pais, 0),
        COALESCE(tb_cad_parceiro_negocio.codigo_cidade, 0),
        COALESCE(tb_cad_parceiro_negocio.codigo_estado, 0),
        COALESCE(pais.nome_pt, ''),
        COALESCE(municipio.nome, ''),
        COALESCE(tb_stc_estados.uf, ''),
		COALESCE(tb_cad_parceiro_negocio.bairro , ''),
		COALESCE(tb_cad_parceiro_negocio.logradouro, ''),
		COALESCE(tb_cad_parceiro_negocio.numero,''),
		COALESCE(tb_cad_parceiro_negocio.cep, ''),
		COALESCE(tb_cad_parceiro_negocio.complemento, '')
    FROM tb_cad_parceiro_negocio
    LEFT JOIN pais 
        ON pais.id = tb_cad_parceiro_negocio.codigo_pais
    LEFT JOIN municipio
        ON municipio.id = tb_cad_parceiro_negocio.codigo_cidade
    LEFT JOIN tb_stc_estados
        ON tb_stc_estados.id = tb_cad_parceiro_negocio.codigo_estado
    WHERE tb_cad_parceiro_negocio.codigo = p_codigo
      AND tb_cad_parceiro_negocio.codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER FUNCTION public.fn_cad_parceiro_negocio_dados(p_codigo integer, p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: fn_cadastro_ativo_select_dados(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_cadastro_ativo_select_dados(p_codigo_empresa integer, p_codigo_ativo integer) RETURNS TABLE(codigo_ativo integer, codigo_parceiro_negocio integer, numero_serie character varying, codigo_fabricante integer, modelo character varying, observacao character varying, data_input date, codigo_empresa integer, descricao text, alias character varying, data_ultima_alteracao date)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY 
	SELECT 
		tb_cad_ativo.codigo,       
		tb_cad_ativo.codigo_cliente,      
		tb_cad_ativo.numero_serie,        
		tb_cad_ativo.codigo_fabricante,        
		tb_cad_ativo.modelo,        
		tb_cad_ativo.observacao,       
		tb_cad_ativo.data_input,      
		tb_cad_ativo.codigo_empresa,       
		tb_cad_ativo.descricao,      
		tb_cad_ativo.alias,       
		tb_cad_ativo.data_ultima_alteracao      
	FROM tb_cad_ativo
	WHERE tb_cad_ativo.codigo = p_codigo_ativo
	AND   tb_cad_ativo.codigo_empresa = p_codigo_empresa
	;
END;
$$;


ALTER FUNCTION public.fn_cadastro_ativo_select_dados(p_codigo_empresa integer, p_codigo_ativo integer) OWNER TO postgres;

--
-- Name: fn_cadastro_ativo_select_foto(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_cadastro_ativo_select_foto(p_codigo_empresa integer, p_codigo_ativo integer) RETURNS TABLE(caminho_completo character varying, titulo character varying, codigo integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY SELECT 
		tb_cad_ativo_foto.caminho_completo, 
		tb_cad_ativo_foto.titulo,
		tb_cad_ativo_foto.codigo
	FROM tb_cad_ativo_foto;
		
END;
$$;


ALTER FUNCTION public.fn_cadastro_ativo_select_foto(p_codigo_empresa integer, p_codigo_ativo integer) OWNER TO postgres;

--
-- Name: fn_cadastro_listar_tecnico(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_cadastro_listar_tecnico(p_codigo_empresa integer) RETURNS TABLE(req_codigo_tecnico integer, req_nome_tecnico character varying, req_codigo_empresa integer, req_ativo boolean, req_data_input date)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
		tb_cad_tecnico.codigo AS req_codigo_tecnico,
		tb_cad_tecnico.nome AS req_nome_tecnico,
		tb_cad_tecnico.codigo_empresa AS req_codigo_empresa,
		tb_cad_tecnico.ativo AS req_ativo,
		tb_cad_tecnico.data_input AS req_data_input
    FROM tb_cad_tecnico
    WHERE tb_cad_tecnico.codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER FUNCTION public.fn_cadastro_listar_tecnico(p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: fn_gen_combo_fabricante_nome_fantasia(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_gen_combo_fabricante_nome_fantasia(p_codigo_empresa integer) RETURNS TABLE(codigo bigint, nome_fantasia character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY SELECT tb_cad_fabricante.codigo, tb_cad_fabricante.nome_fantasia FROM tb_cad_fabricante where codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER FUNCTION public.fn_gen_combo_fabricante_nome_fantasia(p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: fn_gen_combo_parceiros_negocio_razao_social(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_gen_combo_parceiros_negocio_razao_social(p_codigo_empresa integer) RETURNS TABLE(codigo integer, razao_social character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY SELECT tb_cad_parceiro_negocio.codigo, tb_cad_parceiro_negocio.razao_social FROM tb_cad_parceiro_negocio where codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER FUNCTION public.fn_gen_combo_parceiros_negocio_razao_social(p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: fn_gen_filtro_parceiro_negocio(integer, character varying, bigint, bigint, integer, character varying, character varying, character, character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_gen_filtro_parceiro_negocio(p_codigo_empresa integer, p_documento character varying DEFAULT NULL::character varying, p_codigo_pais bigint DEFAULT NULL::bigint, p_codigo_cidade bigint DEFAULT NULL::bigint, p_codigo_estado integer DEFAULT NULL::integer, p_telefone character varying DEFAULT NULL::character varying, p_email character varying DEFAULT NULL::character varying, p_tipo character DEFAULT NULL::bpchar, p_nome_fantasia character varying DEFAULT NULL::character varying, p_razao_social character varying DEFAULT NULL::character varying) RETURNS TABLE(codigo_parceiro integer, documento_parceiro character varying, codigo_uf_parceiro integer, telefone_parceiro character varying, email_parceiro character varying, data_cadastro_parceiro timestamp without time zone, tipo_parceiro character, codigo_empresa integer, nome_fantasia_parceiro character varying, razao_social_parceiro character varying, municipio_parceiro character varying, pais_parceiro character varying, estado_parceiro character varying, codigo_municipio_parceiro integer, codigo_pais_parceiro integer, codigo_estado_parceiro integer, endereco text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
		tb_cad_parceiro_negocio.codigo,
        COALESCE(tb_cad_parceiro_negocio.documento, ''),
        COALESCE(tb_cad_parceiro_negocio.codigo_estado, 0),
        COALESCE(tb_cad_parceiro_negocio.telefone, ''),
        COALESCE(tb_cad_parceiro_negocio.email, ''),
        tb_cad_parceiro_negocio.data_cadastro,
        COALESCE(tb_cad_parceiro_negocio.tipo_parceiro, ''),
        tb_cad_parceiro_negocio.codigo_empresa,
        COALESCE(tb_cad_parceiro_negocio.nome_fantasia, ''),
        COALESCE(tb_cad_parceiro_negocio.razao_social, ''),
        COALESCE(municipio.nome,  ''),
        COALESCE(pais.nome_pt,  ''),
        COALESCE(tb_stc_estados.nome, ''),
        COALESCE(municipio.id,  0),
        COALESCE(pais.id,  0),
        COALESCE(tb_stc_estados.id, 0),
        COALESCE(tb_cad_parceiro_negocio.logradouro, '') || ', ' ||
        COALESCE(tb_cad_parceiro_negocio.numero, '') || ', ' ||
        COALESCE(tb_cad_parceiro_negocio.bairro, '') AS endereco
    FROM tb_cad_parceiro_negocio
    LEFT JOIN municipio 
        ON municipio.id = tb_cad_parceiro_negocio.codigo_cidade
    LEFT JOIN pais
        ON pais.id = tb_cad_parceiro_negocio.codigo_pais
    LEFT JOIN tb_stc_estados
        ON tb_stc_estados.id = tb_cad_parceiro_negocio.codigo_estado
    WHERE (tb_cad_parceiro_negocio.documento ILIKE '%' || p_documento || '%' OR p_documento IS NULL)
      AND (tb_cad_parceiro_negocio.telefone ILIKE '%' || p_telefone || '%' OR p_telefone IS NULL)
      AND (tb_cad_parceiro_negocio.email ILIKE '%' || p_email || '%' OR p_email IS NULL)
      AND (tb_cad_parceiro_negocio.tipo_parceiro = p_tipo OR p_tipo IS NULL)
      AND (tb_cad_parceiro_negocio.nome_fantasia ILIKE '%' || p_nome_fantasia || '%' OR p_nome_fantasia IS NULL)
      AND (tb_cad_parceiro_negocio.razao_social ILIKE '%' || p_razao_social || '%' OR p_razao_social IS NULL)
      AND (tb_cad_parceiro_negocio.codigo_pais = p_codigo_pais OR p_codigo_pais IS NULL)
      AND (tb_cad_parceiro_negocio.codigo_cidade = p_codigo_cidade OR p_codigo_cidade IS NULL)
      AND (tb_cad_parceiro_negocio.codigo_estado = p_codigo_estado OR p_codigo_estado IS NULL);
END;
$$;


ALTER FUNCTION public.fn_gen_filtro_parceiro_negocio(p_codigo_empresa integer, p_documento character varying, p_codigo_pais bigint, p_codigo_cidade bigint, p_codigo_estado integer, p_telefone character varying, p_email character varying, p_tipo character, p_nome_fantasia character varying, p_razao_social character varying) OWNER TO postgres;

--
-- Name: fn_listar_ativos(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_listar_ativos(p_codigo_empresa integer, p_codigo_cliente integer) RETURNS TABLE(codigo integer, codigo_cliente integer, numero_serie character varying, codigo_fabricante integer, modelo character varying, codigo_prioridade smallint, codigo_tecnico_responsavel integer, observacao character varying, data_input date, nivel_manutencao boolean, codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tb_cad_ativo
    WHERE tb_cad_ativo.codigo_empresa = p_codigo_empresa
      AND tb_cad_ativo.codigo_cliente = p_codigo_cliente;
END;
$$;


ALTER FUNCTION public.fn_listar_ativos(p_codigo_empresa integer, p_codigo_cliente integer) OWNER TO postgres;

--
-- Name: fn_listar_fotos_ativo(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_listar_fotos_ativo(p_codigo_ativo integer, p_codigo_empresa integer) RETURNS TABLE(codigo integer, titulo character varying, descricao character varying, caminho_completo character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
        tb_cad_ativo_foto.codigo,
        tb_cad_ativo_foto.titulo,
        tb_cad_ativo_foto.descricao,
        tb_cad_ativo_foto.caminho_completo
    FROM 
        tb_cad_ativo_foto
    WHERE 
        tb_cad_ativo_foto.codigo_ativo = p_codigo_ativo
        AND tb_cad_ativo_foto.codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER FUNCTION public.fn_listar_fotos_ativo(p_codigo_ativo integer, p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: fn_listar_itens(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_listar_itens(p_codigo_empresa integer) RETURNS TABLE(codigo integer, nome_item character varying, preco_base_venda numeric, codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
  RETURN QUERY
  SELECT 
    public.tb_cad_item.codigo,
    public.tb_cad_item.nome_item,
    public.tb_cad_item.preco_base_venda,
    public.tb_cad_item.codigo_empresa
  FROM 
    public.tb_cad_item
  WHERE 
    public.tb_cad_item.codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER FUNCTION public.fn_listar_itens(p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: fn_manutecao_necessidade_select_ativo(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_manutecao_necessidade_select_ativo(p_codigo_empresa integer, p_codigo_nm integer) RETURNS TABLE(get_codigo_ativo_nm bigint, get_codigo_nm bigint, get_codigo_empresa_ativo integer, get_codigo_ativo_referencia bigint, get_descricao_ativo text, get_observacao_ativo text, get_numero_serie_ativo character varying, get_codigo_fabricante_ativo integer, get_modelo_ativo character varying, get_observacao_ativo_referencia character varying, get_nome_fantasia_fabricante character varying, get_razao_social_fabricante character varying, get_descricao_fabricante text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        tb_manutencao_necessidade_ativo.codigo AS get_codigo_ativo_nm,
        tb_manutencao_necessidade_ativo.codigo_necessidade_manutencao AS get_codigo_nm,
        tb_manutencao_necessidade_ativo.codigo_empresa AS get_codigo_empresa_ativo,
        tb_manutencao_necessidade_ativo.codigo_ativo AS get_codigo_ativo_referencia,
        tb_manutencao_necessidade_ativo.descricao AS get_descricao_ativo,
        tb_manutencao_necessidade_ativo.observacao AS get_observacao_ativo,
        tb_cad_ativo.numero_serie AS get_numero_serie_ativo,
        tb_cad_ativo.codigo_fabricante AS get_codigo_fabricante_ativo,
        tb_cad_ativo.modelo AS get_modelo_ativo,
        tb_cad_ativo.observacao AS get_observacao_ativo_referencia,
        tb_cad_fabricante.nome_fantasia AS get_nome_fantasia_fabricante,
        tb_cad_fabricante.razao_social AS get_razao_social_fabricante,
        tb_cad_fabricante.descricao AS get_descricao_fabricante
    FROM tb_manutencao_necessidade_ativo
    LEFT JOIN tb_cad_ativo
        ON tb_cad_ativo.codigo = tb_manutencao_necessidade_ativo.codigo_ativo
        AND tb_cad_ativo.codigo_empresa = tb_manutencao_necessidade_ativo.codigo_empresa
    LEFT JOIN tb_cad_fabricante
        ON tb_cad_fabricante.codigo = tb_cad_ativo.codigo_fabricante
        AND tb_cad_fabricante.codigo_empresa = tb_cad_ativo.codigo_empresa
    WHERE tb_manutencao_necessidade_ativo.codigo_empresa = p_codigo_empresa
    AND tb_manutencao_necessidade_ativo.codigo_necessidade_manutencao = p_codigo_nm;
END;
$$;


ALTER FUNCTION public.fn_manutecao_necessidade_select_ativo(p_codigo_empresa integer, p_codigo_nm integer) OWNER TO postgres;

--
-- Name: fn_manutencao_necessidade_listar_dados_nm(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_manutencao_necessidade_listar_dados_nm(p_codigo_nm integer, p_codigo_empresa integer) RETURNS TABLE(get_codigo_nm bigint, get_codigo_empresa_nm integer, get_solicitante_nm character varying, get_aprovador_nm character varying, get_descricao_nm text, get_observacao_nm text, get_nome_parceiro_negocio_nm character varying, get_nome_contato_nm character varying, get_metodo_contato_nm character varying, get_data_input_nm date, get_data_ultima_alteracao_nm date, get_codigo_tipo_manutencao_nm integer, get_descricao_tipo_manutencao_nm character varying, get_nivel_prioridade_nm integer, get_descricao_nivel_prioridade_nm character varying, get_desconto_bruto_geral numeric, get_acrescimo_bruto_geral numeric, get_codigo_status_nm integer, get_descricao_status_nm character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
        tb_manutencao_necessidade.codigo AS get_codigo_nm,
        tb_manutencao_necessidade.codigo_empresa AS get_codigo_empresa_nm,
        tb_manutencao_necessidade.solicitante AS get_solicitante_nm,
        tb_manutencao_necessidade.aprovador AS get_aprovador_nm,
        tb_manutencao_necessidade.descricao AS get_descricao_nm,
        tb_manutencao_necessidade.observacao AS get_observacao_nm,
        tb_cad_parceiro_negocio.nome_razao_social AS get_nome_parceiro_negocio_nm,
        tb_manutencao_necessidade.nome_contato AS get_nome_contato_nm,
        tb_manutencao_necessidade.metodo_contato AS get_metodo_contato_nm,
        tb_manutencao_necessidade.data_input AS get_data_input_nm,
        tb_manutencao_necessidade.data_ultima_alteracao AS get_data_ultima_alteracao_nm,
        tb_manutencao_necessidade.codigo_stc_tipo_manutencao AS get_codigo_tipo_manutencao_nm,
        tb_stc_tipo_manutencao.descricao AS get_descricao_tipo_manutencao_nm,
        tb_manutencao_necessidade.codigo_stc_nivel_prioridade AS get_nivel_prioridade_nm,
        tb_stc_nivel_prioridade.nivel AS get_descricao_nivel_prioridade_nm,
        tb_manutencao_necessidade.desconto_bruto_geral AS get_desconto_bruto_geral,
        tb_manutencao_necessidade.acrescimo_bruto_geral AS get_acrescimo_bruto_geral,
        tb_manutencao_necessidade.codigo_stc_status_nm AS get_codigo_status_nm,
        tb_stc_status_nm.descricao AS get_descricao_status_nm
    FROM
        tb_manutencao_necessidade
    INNER JOIN tb_cad_parceiro_negocio
        ON tb_cad_parceiro_negocio.codigo = tb_manutencao_necessidade.codigo_parceiro_negocio
        AND tb_cad_parceiro_negocio.codigo_empresa = tb_manutencao_necessidade.codigo_empresa
    LEFT JOIN tb_stc_tipo_manutencao
        ON tb_stc_tipo_manutencao.codigo = tb_manutencao_necessidade.codigo_stc_tipo_manutencao
    LEFT JOIN tb_stc_nivel_prioridade
        ON tb_stc_nivel_prioridade.codigo = tb_manutencao_necessidade.codigo_stc_nivel_prioridade
    LEFT JOIN tb_stc_status_nm
        ON tb_stc_status_nm.codigo = tb_manutencao_necessidade.codigo_stc_status_nm
    WHERE
        tb_manutencao_necessidade.codigo = p_codigo_nm
        AND tb_manutencao_necessidade.codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER FUNCTION public.fn_manutencao_necessidade_listar_dados_nm(p_codigo_nm integer, p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: fn_manutencao_necessidade_select_item_ativo(bigint, bigint, bigint, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_manutencao_necessidade_select_item_ativo(p_codigo bigint, p_codigo_ativo_vinculado bigint, p_codigo_necessidade_manutencao bigint, p_codigo_empresa integer) RETURNS TABLE(get_codigo_ativo_item bigint, get_codigo_ativo_vinculado_item bigint, get_codigo_empresa_ativo_item integer, get_codigo_item_estoque bigint, get_quantidade_item numeric, get_valor_unitario_item numeric, get_tipo_item character, get_descricao_item character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
        tb_manutencao_necessidade_ativo_item.codigo 					AS get_codigo_ativo_item,
        tb_manutencao_necessidade_ativo_item.codigo_ativo_vinculado 	AS get_codigo_ativo_vinculado_item,
        tb_manutencao_necessidade_ativo_item.codigo_empresa 			AS get_codigo_empresa_ativo_item,
        tb_manutencao_necessidade_ativo_item.codigo_item_estoque 		AS get_codigo_item_estoque,
        tb_manutencao_necessidade_ativo_item.quantidade 				AS get_quantidade_item,
        tb_manutencao_necessidade_ativo_item.valor_unitario 			AS get_valor_unitario_item,
        tb_manutencao_necessidade_ativo_item.tipo 						AS get_tipo_item,
		tb_cad_item.nome_item											AS get_descricao_item
    FROM 
        tb_manutencao_necessidade_ativo_item
		LEFT JOIN tb_cad_item
		ON 	tb_cad_item.codigo = tb_manutencao_necessidade_ativo_item.codigo_item_estoque
		AND	tb_cad_item.codigo_empresa = tb_manutencao_necessidade_ativo_item.codigo_empresa
    WHERE 
        tb_manutencao_necessidade_ativo_item.codigo = p_codigo 
		AND tb_manutencao_necessidade_ativo_item.codigo_ativo_vinculado = p_codigo_ativo_vinculado 
		AND tb_manutencao_necessidade_ativo_item.codigo_necessidade_manutencao = p_codigo_necessidade_manutencao 
		AND tb_manutencao_necessidade_ativo_item.codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER FUNCTION public.fn_manutencao_necessidade_select_item_ativo(p_codigo bigint, p_codigo_ativo_vinculado bigint, p_codigo_necessidade_manutencao bigint, p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: fn_necessidade_manutencao_load_dados(bigint, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_necessidade_manutencao_load_dados(p_codigo_nm bigint, p_codigo_empresa_nm integer) RETURNS TABLE(get_codigo_nm bigint, get_codigo_empresa integer, get_solicitante character varying, get_aprovador character varying, get_descricao_nm text, get_observacao_nm text, get_codigo_parceiro_negocio integer, get_razao_social character varying, get_is_cnpj boolean, get_documento_parceiro character varying, get_endereco_parceiro character varying, get_cidade_parceiro character varying, get_estado_parceiro character varying, get_cep_parceiro character varying, get_telefone_parceiro character varying, get_email_parceiro character varying, get_nome_contato character varying, get_metodo_contato character varying, get_data_input_nm date, get_data_ultima_alteracao_nm date, get_codigo_usuario_ultima_alteracao integer, get_nome_usuario_alterado_por character varying, get_codigo_tipo_manutencao integer, get_descricao_tipo_manutencao character varying, get_codigo_nivel_prioridade integer, get_nivel_descricao character varying, get_desconto_bruto_geral numeric, get_acrescimo_bruto_geral numeric, get_codigo_status_nm integer, get_descricao_status character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
  RETURN QUERY
  SELECT 
  	tb_manutencao_necessidade.codigo AS get_codigo_nm,
  	tb_manutencao_necessidade.codigo_empresa AS get_codigo_empresa_nm,
  	tb_manutencao_necessidade.solicitante AS get_solicitante,
  	tb_manutencao_necessidade.aprovador AS get_aprovador,
  	tb_manutencao_necessidade.descricao AS get_descricao_nm,
  	tb_manutencao_necessidade.observacao AS get_observacao_nm,
	--JOINS parceiro
  	tb_manutencao_necessidade.codigo_parceiro_negocio AS get_codigo_parceiro_negocio,
  	tb_cad_parceiro_negocio.nome_razao_social AS get_razao_social,
  	tb_cad_parceiro_negocio.is_cnpj AS get_is_cnpj,
  	tb_cad_parceiro_negocio.documento AS get_documento_parceiro,
  	tb_cad_parceiro_negocio.endereco AS get_endereco_parceiro,
  	tb_cad_parceiro_negocio.cidade AS get_cidade_parceiro,
  	tb_cad_parceiro_negocio.estado::VARCHAR(2) AS get_estado_parceiro,
  	tb_cad_parceiro_negocio.cep AS get_cep_parceiro,
  	tb_cad_parceiro_negocio.telefone AS get_telefone_parceiro,
  	tb_cad_parceiro_negocio.email AS get_email_parceiro,
	--FIM dos JOINS parceiro
  	tb_manutencao_necessidade.nome_contato AS get_nome_contato,
  	tb_manutencao_necessidade.metodo_contato AS get_metodo_contato,
  	tb_manutencao_necessidade.data_input AS get_data_input_nm,
  	tb_manutencao_necessidade.data_ultima_alteracao AS get_data_ultima_alteracao_nm,
	--JOIN usuário
  	tb_manutencao_necessidade.codigo_usuario_ultima_alteracao AS get_codigo_usuario_ultima_alteracao,
  	tb_cad_usuario.usuario AS get_nome_usuario_alterado_por,
	--JOIN tipo manutencao
  	tb_manutencao_necessidade.codigo_stc_tipo_manutencao AS get_codigo_tipo_manutencao,
  	tb_stc_tipo_manutencao.descricao AS get_descricao_tipo_manutencao,
	--JOIN nivel prioridade
    tb_manutencao_necessidade.codigo_stc_nivel_prioridade AS get_codigo_nivel_prioridade,
	tb_stc_nivel_prioridade.nivel AS get_nivel_descricao,
    tb_manutencao_necessidade.desconto_bruto_geral AS get_desconto_bruto_geral,
    tb_manutencao_necessidade.acrescimo_bruto_geral AS get_acrescimo_bruto_geral,
	--JOIN status
    tb_manutencao_necessidade.codigo_stc_status_nm AS get_codigo_status_nm,
	tb_stc_status_nm.descricao AS get_descricao_status
  FROM 
    tb_manutencao_necessidade
	INNER JOIN tb_cad_parceiro_negocio
		ON 	tb_cad_parceiro_negocio.codigo = tb_manutencao_necessidade.codigo_parceiro_negocio
		AND	tb_manutencao_necessidade.codigo_empresa = tb_manutencao_necessidade.codigo_empresa
	INNER JOIN tb_cad_usuario
		ON tb_cad_usuario.codigo = tb_manutencao_necessidade.codigo_usuario_ultima_alteracao
		AND tb_cad_usuario.codigo_empresa = tb_manutencao_necessidade.codigo_empresa
	INNER JOIN tb_stc_tipo_manutencao 
		ON tb_stc_tipo_manutencao.codigo = tb_manutencao_necessidade.codigo_stc_tipo_manutencao
	INNER JOIN tb_stc_nivel_prioridade
		ON tb_stc_nivel_prioridade.codigo = tb_manutencao_necessidade.codigo_stc_nivel_prioridade
	INNER JOIN tb_stc_status_nm
		ON tb_stc_status_nm.codigo = tb_manutencao_necessidade.codigo_stc_status_nm
  WHERE tb_manutencao_necessidade.codigo = p_codigo_nm
    AND tb_manutencao_necessidade.codigo_empresa = p_codigo_empresa_nm;

END;
$$;


ALTER FUNCTION public.fn_necessidade_manutencao_load_dados(p_codigo_nm bigint, p_codigo_empresa_nm integer) OWNER TO postgres;

--
-- Name: fn_ordem_servico_load_dados(bigint, bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_ordem_servico_load_dados(p_codigo_empresa bigint, p_codigo_os bigint) RETURNS TABLE(codigo integer, codigo_empresa integer, codigo_parceiro_negocio integer, codigo_ativo integer, observacao character varying, data_criacao date, data_ultima_alteracao date, codigo_usuario_ultima_alteracao bigint, nome_razao_social character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
  RETURN QUERY
  SELECT 
   	tb_manutencao_ordem_servico.codigo,
    tb_manutencao_ordem_servico.codigo_empresa,
    tb_manutencao_ordem_servico.codigo_parceiro_negocio,
    tb_manutencao_ordem_servico.codigo_ativo,
	tb_manutencao_ordem_servico.observacao,
    tb_manutencao_ordem_servico.data_criacao,
    tb_manutencao_ordem_servico.data_ultima_alteracao,
	tb_manutencao_ordem_servico.codigo_usuario_ultima_alteracao,
	tb_cad_parceiro_negocio.nome_razao_social
  FROM 
    tb_manutencao_ordem_servico
	INNER JOIN tb_cad_parceiro_negocio
	ON 	tb_cad_parceiro_negocio.codigo = tb_manutencao_ordem_servico.codigo_parceiro_negocio
	AND	tb_cad_parceiro_negocio.codigo_empresa = tb_manutencao_ordem_servico.codigo_empresa;

END;
$$;


ALTER FUNCTION public.fn_ordem_servico_load_dados(p_codigo_empresa bigint, p_codigo_os bigint) OWNER TO postgres;

--
-- Name: fn_ordem_servico_load_item(bigint, bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_ordem_servico_load_item(p_codigo_empresa bigint, p_codigo_os bigint) RETURNS TABLE(codigo integer, codigo_empresa integer, codigo_ordem_servico integer, codigo_item integer, nome_item character varying, quantidade double precision, valor_unitario double precision)
    LANGUAGE plpgsql
    AS $$
BEGIN
  RETURN QUERY
  SELECT 
   	tb_manutencao_ordem_servico_item.codigo ,
    tb_manutencao_ordem_servico_item.codigo_empresa ,
    tb_manutencao_ordem_servico_item.codigo_ordem_servico ,
    tb_manutencao_ordem_servico_item.codigo_item ,
	tb_cad_item.nome_item ,
    tb_manutencao_ordem_servico_item.quantidade ,
    tb_manutencao_ordem_servico_item.valor_unitario 
  FROM 
    tb_manutencao_ordem_servico_item
	INNER JOIN tb_cad_item
	ON 	tb_cad_item.codigo = tb_manutencao_ordem_servico_item.codigo_item
	AND	tb_cad_item.codigo_empresa = tb_manutencao_ordem_servico_item.codigo_empresa
  WHERE tb_manutencao_ordem_servico_item.codigo_empresa = p_codigo_empresa
	AND tb_manutencao_ordem_servico_item.codigo_ordem_servico = p_codigo_os;
END;
$$;


ALTER FUNCTION public.fn_ordem_servico_load_item(p_codigo_empresa bigint, p_codigo_os bigint) OWNER TO postgres;

--
-- Name: fn_select_cad_ativo_dados(integer, integer, integer, character varying, integer, character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_select_cad_ativo_dados(p_codigo_empresa integer, p_codigo integer DEFAULT NULL::integer, p_codigo_cliente integer DEFAULT NULL::integer, p_numero_serie character varying DEFAULT NULL::character varying, p_codigo_fabricante integer DEFAULT NULL::integer, p_modelo character varying DEFAULT NULL::character varying, p_observacao character varying DEFAULT NULL::character varying) RETURNS TABLE(codigo_ativo integer, codigo_cliente_ativo integer, numero_serie_ativo character varying, codigo_fabricante_ativo integer, modelo_ativo character varying, observacao_ativo character varying, data_input_ativo date, codigo_empresa_ativo integer, nome_fantasia_parceiro character varying, contato_parceiro character varying, nome_fantasia_fabricante character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
        tb_cad_ativo.codigo, 
        tb_cad_ativo.codigo_cliente, 
        tb_cad_ativo.numero_serie,
        tb_cad_ativo.codigo_fabricante,
        tb_cad_ativo.modelo,
        tb_cad_ativo.observacao,
        tb_cad_ativo.data_input,
        tb_cad_ativo.codigo_empresa,
		tb_cad_parceiro_negocio.nome_fantasia,
		tb_cad_parceiro_negocio.contato,
		tb_cad_fabricante.nome_fantasia
    FROM tb_cad_ativo
	LEFT JOIN tb_cad_parceiro_negocio
		ON	tb_cad_parceiro_negocio.codigo = tb_cad_ativo.codigo_cliente
		AND	tb_cad_parceiro_negocio.codigo_empresa = tb_cad_ativo.codigo_empresa
	LEFT JOIN tb_cad_fabricante
		ON	tb_cad_fabricante.codigo = tb_cad_ativo.codigo_fabricante
		AND	tb_cad_fabricante.codigo_empresa = tb_cad_ativo.codigo_empresa
	
    WHERE (tb_cad_ativo.codigo = p_codigo OR p_codigo IS NULL)
      AND (tb_cad_ativo.codigo_cliente = p_codigo_cliente OR p_codigo_cliente IS NULL)
      AND (tb_cad_ativo.numero_serie LIKE '%' || p_numero_serie || '%' OR p_numero_serie IS NULL)
      AND (tb_cad_ativo.codigo_fabricante = p_codigo_fabricante OR p_codigo_fabricante IS NULL)
      AND (tb_cad_ativo.modelo LIKE '%' || p_modelo || '%' OR p_modelo IS NULL)
      AND (tb_cad_ativo.observacao LIKE '%' || p_observacao || '%' OR p_observacao IS NULL)
      AND tb_cad_ativo.codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER FUNCTION public.fn_select_cad_ativo_dados(p_codigo_empresa integer, p_codigo integer, p_codigo_cliente integer, p_numero_serie character varying, p_codigo_fabricante integer, p_modelo character varying, p_observacao character varying) OWNER TO postgres;

--
-- Name: fn_select_estados(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_select_estados() RETURNS TABLE(codigo_estado integer, uf character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY SELECT 
	tb_stc_estados.id, 
	tb_stc_estados.uf 
	FROM tb_stc_estados;
END;
$$;


ALTER FUNCTION public.fn_select_estados() OWNER TO postgres;

--
-- Name: fn_select_municipios(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_select_municipios(p_uf character varying) RETURNS TABLE(codigo_municipio integer, nome_municipio character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY SELECT id, nome FROM municipio WHERE uf = p_uf;
END;
$$;


ALTER FUNCTION public.fn_select_municipios(p_uf character varying) OWNER TO postgres;

--
-- Name: fn_select_paises(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_select_paises() RETURNS TABLE(codigo_pais integer, nome_pais character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY SELECT id, nome_pt FROM pais;
END;
$$;


ALTER FUNCTION public.fn_select_paises() OWNER TO postgres;

--
-- Name: sp_atualizar_parceiro_negocio(integer, integer, character varying, character varying, character varying, character varying, character varying, character varying, character, bigint, integer, bigint, boolean, character varying, character varying, character varying, character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_atualizar_parceiro_negocio(IN p_codigo integer, IN p_codigo_empresa integer, IN p_documento character varying, IN p_nome_fantasia character varying, IN p_razao_social character varying, IN p_email character varying, IN p_contato character varying, IN p_telefone character varying, IN p_tipo character, IN p_codigo_pais bigint, IN p_codigo_estado integer, IN p_codigo_cidade bigint, IN p_tipo_documento boolean, IN p_cep character varying, IN p_logradouro character varying, IN p_numero character varying, IN p_complemento character varying, IN p_bairro character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN

    UPDATE public.tb_cad_parceiro_negocio
    SET 
		documento = p_documento,
		nome_fantasia = p_nome_fantasia,
		razao_social = p_razao_social,
		email = p_email,
		contato = p_contato,
		telefone = p_telefone,
		tipo_parceiro = p_tipo,
		codigo_pais = p_codigo_pais,
		codigo_estado = p_codigo_estado,
		codigo_cidade = p_codigo_cidade,
		is_cnpj = p_tipo_documento,
		cep = p_cep,
		logradouro = p_logradouro,
		numero  = p_numero, 
		complemento  = p_complemento,
		bairro = p_bairro
		
    WHERE 
        codigo = p_codigo 
        AND codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER PROCEDURE public.sp_atualizar_parceiro_negocio(IN p_codigo integer, IN p_codigo_empresa integer, IN p_documento character varying, IN p_nome_fantasia character varying, IN p_razao_social character varying, IN p_email character varying, IN p_contato character varying, IN p_telefone character varying, IN p_tipo character, IN p_codigo_pais bigint, IN p_codigo_estado integer, IN p_codigo_cidade bigint, IN p_tipo_documento boolean, IN p_cep character varying, IN p_logradouro character varying, IN p_numero character varying, IN p_complemento character varying, IN p_bairro character varying) OWNER TO postgres;

--
-- Name: sp_cadastro_ativo_delete_foto(integer, integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_cadastro_ativo_delete_foto(IN p_codigo_empresa integer, IN p_codigo_ativo integer, IN p_codigo integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM tb_cad_ativo_foto
    WHERE 	codigo = p_codigo 
	AND 	codigo_ativo = p_codigo_ativo
    AND 	codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER PROCEDURE public.sp_cadastro_ativo_delete_foto(IN p_codigo_empresa integer, IN p_codigo_ativo integer, IN p_codigo integer) OWNER TO postgres;

--
-- Name: sp_cadastro_basico_ambiente(character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_cadastro_basico_ambiente(IN nome_ambiente character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO tb_ambientes (nome_ambiente)
    VALUES (nome_ambiente);
END;
$$;


ALTER PROCEDURE public.sp_cadastro_basico_ambiente(IN nome_ambiente character varying) OWNER TO postgres;

--
-- Name: sp_cadastro_delete_tecnico(integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_cadastro_delete_tecnico(IN p_codigo_tecnico integer, IN p_codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
BEGIN

	DELETE FROM tb_cad_tecnico
	WHERE 	codigo = p_codigo_tecnico
	AND		codigo_empresa = p_codigo_empresa;
 
END;
$$;


ALTER PROCEDURE public.sp_cadastro_delete_tecnico(IN p_codigo_tecnico integer, IN p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: sp_cadastro_upsert_tecnico(integer, integer, character varying, boolean); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_cadastro_upsert_tecnico(IN p_codigo_tecnico integer, IN p_codigo_empresa integer, IN p_nome character varying, IN p_ativo boolean)
    LANGUAGE plpgsql
    AS $$
BEGIN

  -- Faz o UPSERT: insere um novo item ou atualiza o existente com base no conflito
  INSERT INTO tb_cad_tecnico (
    codigo,
    codigo_empresa,
    nome,
    ativo,
	data_input
  ) VALUES (
    p_codigo_tecnico,
    p_codigo_empresa,
    p_nome,
    p_ativo, 
    now()
  )
  ON CONFLICT (codigo, codigo_empresa)
  DO UPDATE SET
    nome       = EXCLUDED.nome,
    ativo 	   = EXCLUDED.ativo;
END;
$$;


ALTER PROCEDURE public.sp_cadastro_upsert_tecnico(IN p_codigo_tecnico integer, IN p_codigo_empresa integer, IN p_nome character varying, IN p_ativo boolean) OWNER TO postgres;

--
-- Name: sp_carregar_empresas(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_carregar_empresas() RETURNS TABLE(codigo integer, nome_fantasia character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY SELECT tb_info_empresa.codigo, tb_info_empresa.nome_fantasia FROM public.tb_info_empresa;
END;
$$;


ALTER FUNCTION public.sp_carregar_empresas() OWNER TO postgres;

--
-- Name: sp_deletar_parceiro_negocio(integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_deletar_parceiro_negocio(IN p_codigo integer, IN p_codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM public.tb_cad_parceiro_negocio
    WHERE 
        codigo = p_codigo 
        AND codigo_empresa = p_codigo_empresa;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Parceiro de negócio não encontrado para exclusão.';
    END IF;
END;
$$;


ALTER PROCEDURE public.sp_deletar_parceiro_negocio(IN p_codigo integer, IN p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: sp_delete_cadastro_basico_ativo(integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_delete_cadastro_basico_ativo(IN p_codigo integer, IN p_codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
BEGIN

    DELETE FROM tb_cad_ativo
    WHERE codigo = p_codigo
    AND codigo_empresa = p_codigo_empresa;
   
END;
$$;


ALTER PROCEDURE public.sp_delete_cadastro_basico_ativo(IN p_codigo integer, IN p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: sp_delete_cadastro_parceiro_negocio(integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_delete_cadastro_parceiro_negocio(IN p_codigo_empresa integer, IN p_codigo_parceiro integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Deleta a foto da tabela
    DELETE FROM tb_cad_parceiro_negocio
    WHERE codigo = p_codigo_parceiro
    AND codigo_empresa = p_codigo_empresa;
    
END;
$$;


ALTER PROCEDURE public.sp_delete_cadastro_parceiro_negocio(IN p_codigo_empresa integer, IN p_codigo_parceiro integer) OWNER TO postgres;

--
-- Name: sp_insert_cadastro_basico_ativo(integer, character varying, integer, character varying, character varying, integer, text, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_insert_cadastro_basico_ativo(IN p_codigo_cliente integer, IN p_numero_serie character varying, IN p_codigo_fabricante integer, IN p_modelo character varying, IN p_observacao character varying, IN p_codigo_empresa integer, IN p_descricao text, IN p_alias text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_codigo integer;
BEGIN
    SELECT COALESCE(MAX(codigo), 0) + 1 INTO v_codigo FROM tb_cad_ativo WHERE codigo_empresa = p_codigo_empresa;

    INSERT INTO tb_cad_ativo (
        codigo,
        codigo_cliente,
        numero_serie,
        codigo_fabricante,
        modelo,
        observacao,
        data_input,
        codigo_empresa,
		descricao,
		alias,
		data_ultima_alteracao
    ) VALUES (
        v_codigo,
        p_codigo_cliente,
        p_numero_serie,
        p_codigo_fabricante,
        p_modelo,
        p_observacao,
        CURRENT_DATE,
        p_codigo_empresa,
		p_descricao,
		p_alias,
		CURRENT_DATE
    );
END;
$$;


ALTER PROCEDURE public.sp_insert_cadastro_basico_ativo(IN p_codigo_cliente integer, IN p_numero_serie character varying, IN p_codigo_fabricante integer, IN p_modelo character varying, IN p_observacao character varying, IN p_codigo_empresa integer, IN p_descricao text, IN p_alias text) OWNER TO postgres;

--
-- Name: sp_insert_cadastro_basico_ativo_foto(integer, integer, character varying, character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_insert_cadastro_basico_ativo_foto(IN p_codigo_ativo integer, IN p_codigo_empresa integer, IN p_titulo character varying, IN p_caminho_completo character varying, IN p_descricao character varying DEFAULT NULL::character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_codigo_foto integer;
BEGIN
    -- Gerar o próximo valor para o código único da foto
    SELECT COALESCE(MAX(codigo), 0) + 1 INTO v_codigo_foto FROM tb_cad_ativo_foto 
    WHERE codigo_empresa = p_codigo_empresa;

    -- Inserir novo registro de foto vinculado ao ativo
    INSERT INTO tb_cad_ativo_foto (
        codigo,
        codigo_ativo,
        codigo_empresa,
        titulo,
        caminho_completo,
        data_upload,
        descricao
    ) VALUES (
        v_codigo_foto,
        p_codigo_ativo,
        p_codigo_empresa,
        p_titulo,
        p_caminho_completo,
        CURRENT_TIMESTAMP,
        p_descricao
    );
END;
$$;


ALTER PROCEDURE public.sp_insert_cadastro_basico_ativo_foto(IN p_codigo_ativo integer, IN p_codigo_empresa integer, IN p_titulo character varying, IN p_caminho_completo character varying, IN p_descricao character varying) OWNER TO postgres;

--
-- Name: sp_insert_cadastro_basico_item_estoque(character varying, numeric, numeric, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_insert_cadastro_basico_item_estoque(IN p_nome_item character varying, IN p_preco_base_venda numeric, IN p_custo numeric, IN p_codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_codigo INTEGER;  -- Declarando v_codigo como INTEGER
BEGIN
  -- Obter o próximo valor de código para o item, dentro do contexto da empresa
  SELECT COALESCE(MAX(codigo), 0) + 1 INTO v_codigo
  FROM tb_cad_item
  WHERE codigo_empresa = p_codigo_empresa;

  -- Inserir o novo item de estoque
  INSERT INTO tb_cad_item (codigo, nome_item, preco_base_venda, custo, codigo_empresa, data_input)
  VALUES (v_codigo, p_nome_item, p_preco_base_venda, p_custo, p_codigo_empresa, NOW());

END;
$$;


ALTER PROCEDURE public.sp_insert_cadastro_basico_item_estoque(IN p_nome_item character varying, IN p_preco_base_venda numeric, IN p_custo numeric, IN p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: sp_insert_cadastro_basico_parceiro_negocio(integer, character varying, character varying, character varying, character varying, timestamp without time zone, character, character varying, character varying, character varying, character varying, character varying, character varying, character varying, bigint, bigint, integer, boolean); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_insert_cadastro_basico_parceiro_negocio(IN p_codigo_empresa integer, IN p_documento character varying, IN p_cep character varying, IN p_telefone character varying, IN p_email character varying, IN p_data_cadastro timestamp without time zone, IN p_tipo character, IN p_nome_fantasia character varying, IN p_razao_social character varying, IN p_logradouro character varying, IN p_numero character varying, IN p_complemento character varying, IN p_bairro character varying, IN p_contato character varying, IN p_codigo_pais bigint, IN p_codigo_cidade bigint, IN p_codigo_estado integer, IN p_tipo_documento boolean)
    LANGUAGE plpgsql
    AS $$
DECLARE
    max_codigo INTEGER;
BEGIN

	SELECT COALESCE(MAX(codigo), 0) +1 INTO max_codigo 
    FROM tb_cad_parceiro_negocio 
    WHERE codigo_empresa = p_codigo_empresa;


	INSERT INTO tb_cad_parceiro_negocio (
    codigo,
	codigo_empresa,
	documento,
	cep,
	telefone,
	email,
	data_cadastro,
	tipo_parceiro,
	nome_fantasia,
	razao_social,
	logradouro,
	numero,
	complemento,
	bairro,
	contato,
	codigo_pais,
	codigo_cidade,
	codigo_estado,
	is_cnpj
    ) VALUES (
	max_codigo,
	p_codigo_empresa,
	p_documento,
	p_cep,
	p_telefone,
	p_email,
	p_data_cadastro,
	p_tipo,
	p_nome_fantasia,
	p_razao_social,
	p_logradouro,
	p_numero,
	p_complemento,
	p_bairro,
	p_contato,
	p_codigo_pais,
	p_codigo_cidade,
	p_codigo_estado,
	p_tipo_documento
    );
END;
$$;


ALTER PROCEDURE public.sp_insert_cadastro_basico_parceiro_negocio(IN p_codigo_empresa integer, IN p_documento character varying, IN p_cep character varying, IN p_telefone character varying, IN p_email character varying, IN p_data_cadastro timestamp without time zone, IN p_tipo character, IN p_nome_fantasia character varying, IN p_razao_social character varying, IN p_logradouro character varying, IN p_numero character varying, IN p_complemento character varying, IN p_bairro character varying, IN p_contato character varying, IN p_codigo_pais bigint, IN p_codigo_cidade bigint, IN p_codigo_estado integer, IN p_tipo_documento boolean) OWNER TO postgres;

--
-- Name: sp_manutencao_necessidade_insert_ativo_imagem(bigint, bigint, integer, character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_manutencao_necessidade_insert_ativo_imagem(IN p_codigo_ativo_vinculado bigint, IN p_codigo_nm bigint, IN p_codigo_empresa integer, IN p_titulo character varying, IN p_caminho character varying)
    LANGUAGE plpgsql
    AS $$

DECLARE
 v_codigo_foto integer;
BEGIN
 	SELECT COALESCE(MAX(codigo), 0) + 1 INTO v_codigo_foto FROM tb_manutencao_necessidade_ativo_imagem 
    WHERE codigo_empresa = p_codigo_empresa;

  INSERT INTO tb_manutencao_necessidade_ativo_imagem (
    codigo,
	codigo_ativo_vinculado,
	codigo_necessidade_manutencao,
	codigo_empresa,
	titulo,
	caminho_completo,
	data_input
  ) VALUES (
    v_codigo_foto,
	p_codigo_ativo_vinculado,
	p_codigo_nm,
	p_codigo_empresa,
	p_titulo,
	p_caminho,
	now()
  );
END;
$$;


ALTER PROCEDURE public.sp_manutencao_necessidade_insert_ativo_imagem(IN p_codigo_ativo_vinculado bigint, IN p_codigo_nm bigint, IN p_codigo_empresa integer, IN p_titulo character varying, IN p_caminho character varying) OWNER TO postgres;

--
-- Name: sp_manutencao_ordem_servico_delete_item(bigint, integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_manutencao_ordem_servico_delete_item(IN p_codigo_item bigint, IN p_codigo_empresa integer, IN p_codigo_os integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM tb_manutencao_ordem_servico_item
    WHERE 	codigo = p_codigo_item 
    AND 	codigo_empresa = p_codigo_empresa
	AND 	codigo_ordem_servico = p_codigo_os;
END;
$$;


ALTER PROCEDURE public.sp_manutencao_ordem_servico_delete_item(IN p_codigo_item bigint, IN p_codigo_empresa integer, IN p_codigo_os integer) OWNER TO postgres;

--
-- Name: sp_manutencao_ordem_servico_delete_os(integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_manutencao_ordem_servico_delete_os(IN p_codigo_empresa integer, IN p_codigo_os integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM tb_manutencao_ordem_servico
    WHERE 	codigo = p_codigo_os 
    AND 	codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER PROCEDURE public.sp_manutencao_ordem_servico_delete_os(IN p_codigo_empresa integer, IN p_codigo_os integer) OWNER TO postgres;

--
-- Name: sp_manutencao_ordem_servico_upsert_item(integer, bigint, bigint, integer, double precision, numeric, bigint); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_manutencao_ordem_servico_upsert_item(IN p_codigo_empresa integer, IN p_codigo_os bigint, IN p_codigo_item bigint, IN p_codigo_item_estoque integer, IN p_valor_unitario double precision, IN p_quantidade numeric, IN p_codigo_usuario_ultima_alteracao bigint)
    LANGUAGE plpgsql
    AS $$
BEGIN

  -- Faz o UPSERT: insere um novo item ou atualiza o existente com base no conflito
  INSERT INTO tb_manutencao_ordem_servico_item (
    codigo,
    codigo_empresa,
    codigo_ordem_servico,
    codigo_item,
    quantidade,
    valor_unitario,
    data_ultima_alteracao,
    codigo_usuario_ultima_alteracao
  ) VALUES (
    p_codigo_item,
    p_codigo_empresa,
    p_codigo_os,
    p_codigo_item_estoque, -- Corrigido para usar p_codigo_item em vez de p_codigo_item_estoque
    p_quantidade,
    p_valor_unitario,
    NOW(),
    p_codigo_usuario_ultima_alteracao
  )
  ON CONFLICT (codigo, codigo_empresa, codigo_ordem_servico)
  DO UPDATE SET
    codigo_item                       = EXCLUDED.codigo_item,
    quantidade                        = EXCLUDED.quantidade,
    valor_unitario                    = EXCLUDED.valor_unitario,
    data_ultima_alteracao             = NOW(),
    codigo_usuario_ultima_alteracao   = EXCLUDED.codigo_usuario_ultima_alteracao;

END;
$$;


ALTER PROCEDURE public.sp_manutencao_ordem_servico_upsert_item(IN p_codigo_empresa integer, IN p_codigo_os bigint, IN p_codigo_item bigint, IN p_codigo_item_estoque integer, IN p_valor_unitario double precision, IN p_quantidade numeric, IN p_codigo_usuario_ultima_alteracao bigint) OWNER TO postgres;

--
-- Name: sp_necessidade_manutencao_delete_ativo_item_nm(bigint, bigint, bigint, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_necessidade_manutencao_delete_ativo_item_nm(IN p_codigo_ativo_item bigint, IN p_codigo_ativo_vinculado bigint, IN p_codigo_necessidade_manutencao bigint, IN p_codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM tb_manutencao_necessidade_ativo_item
    WHERE 	codigo = p_codigo_ativo_item 
	AND 	codigo_ativo_vinculado = p_codigo_ativo_vinculado
	AND 	codigo_necessidade_manutencao = p_codigo_necessidade_manutencao
    AND 	codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER PROCEDURE public.sp_necessidade_manutencao_delete_ativo_item_nm(IN p_codigo_ativo_item bigint, IN p_codigo_ativo_vinculado bigint, IN p_codigo_necessidade_manutencao bigint, IN p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: sp_necessidade_manutencao_delete_ativo_nm(bigint, integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_necessidade_manutencao_delete_ativo_nm(IN p_codigo_ativo bigint, IN p_codigo_nm integer, IN p_codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM tb_manutencao_necessidade_ativo
    WHERE 	codigo = p_codigo_ativo 
	AND 	codigo_necessidade_manutencao = p_codigo_nm
    AND 	codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER PROCEDURE public.sp_necessidade_manutencao_delete_ativo_nm(IN p_codigo_ativo bigint, IN p_codigo_nm integer, IN p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: sp_necessidade_manutencao_delete_nm(integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_necessidade_manutencao_delete_nm(IN p_codigo_nm integer, IN p_codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM tb_manutencao_necessidade
    WHERE 	codigo = p_codigo_nm 
    AND 	codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER PROCEDURE public.sp_necessidade_manutencao_delete_nm(IN p_codigo_nm integer, IN p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: sp_necessidade_manutencao_insert(integer, character varying, character varying, character varying, integer, character varying, character varying, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_necessidade_manutencao_insert(IN p_codigo_empresa integer, IN p_solicitante character varying, IN p_descricao character varying, IN p_observacao character varying, IN p_codigo_parceiro_negocio integer, IN p_nome_contato character varying, IN p_metodo_contato character varying, IN p_codigo_usuario integer)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_codigo integer;
BEGIN
    SELECT COALESCE(MAX(codigo), 0) + 1 INTO v_codigo FROM tb_manutencao_necessidade WHERE codigo_empresa = p_codigo_empresa;

    INSERT INTO tb_manutencao_necessidade (
        codigo,
        codigo_empresa,
        solicitante,
		descricao,
		observacao,
		codigo_parceiro_negocio,
		nome_contato,
		metodo_contato,
		data_input,
		codigo_usuario_ultima_alteracao
    ) VALUES (
		v_codigo,
		p_codigo_empresa,
		p_solicitante,
		p_descricao,
		p_observacao,
		p_codigo_parceiro_negocio,
		p_nome_contato,
		p_metodo_contato,
		now(),
		p_codigo_usuario
    );
END;
$$;


ALTER PROCEDURE public.sp_necessidade_manutencao_insert(IN p_codigo_empresa integer, IN p_solicitante character varying, IN p_descricao character varying, IN p_observacao character varying, IN p_codigo_parceiro_negocio integer, IN p_nome_contato character varying, IN p_metodo_contato character varying, IN p_codigo_usuario integer) OWNER TO postgres;

--
-- Name: sp_necessidade_manutencao_update(bigint, integer, character varying, character varying, text, text, integer, character varying, character varying, integer, integer, integer, numeric, numeric); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_necessidade_manutencao_update(IN p_codigo_nm bigint, IN p_codigo_empresa integer, IN p_solicitante character varying, IN p_aprovador character varying, IN p_descricao text, IN p_observacao text, IN p_codigo_parceiro_negocio integer, IN p_nome_contato character varying, IN p_metodo_contato character varying, IN p_codigo_usuario integer, IN p_codigo_tipo_manutencao integer, IN p_codigo_nivel_prioridade integer, IN p_desconto_bruto_geral numeric, IN p_acrescimo_bruto_geral numeric)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE tb_manutencao_necessidade
    SET 
		solicitante = p_solicitante,
		aprovador = p_aprovador,
		descricao = p_descricao,
		observacao = p_observacao,
		codigo_parceiro_negocio = p_codigo_parceiro_negocio,
		nome_contato = p_nome_contato,
		metodo_contato = p_metodo_contato,
		data_ultima_alteracao = NOW(),
		codigo_usuario_ultima_alteracao = p_codigo_usuario,
		codigo_stc_tipo_manutencao = p_codigo_tipo_manutencao,
		codigo_stc_nivel_prioridade = p_codigo_nivel_prioridade,
		desconto_bruto_geral = p_desconto_bruto_geral,
		acrescimo_bruto_geral = p_acrescimo_bruto_geral
    WHERE codigo = p_codigo_nm
	AND	codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER PROCEDURE public.sp_necessidade_manutencao_update(IN p_codigo_nm bigint, IN p_codigo_empresa integer, IN p_solicitante character varying, IN p_aprovador character varying, IN p_descricao text, IN p_observacao text, IN p_codigo_parceiro_negocio integer, IN p_nome_contato character varying, IN p_metodo_contato character varying, IN p_codigo_usuario integer, IN p_codigo_tipo_manutencao integer, IN p_codigo_nivel_prioridade integer, IN p_desconto_bruto_geral numeric, IN p_acrescimo_bruto_geral numeric) OWNER TO postgres;

--
-- Name: sp_necessidade_manutencao_upsert_ativo(bigint, bigint, integer, bigint, text, text); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_necessidade_manutencao_upsert_ativo(IN p_codigo bigint, IN p_codigo_necessidade_manutencao bigint, IN p_codigo_empresa integer, IN p_codigo_ativo bigint, IN p_descricao text, IN p_observacao text)
    LANGUAGE plpgsql
    AS $$
BEGIN

  -- Faz o UPSERT: insere um novo item ou atualiza o existente com base no conflito
  INSERT INTO tb_manutencao_necessidade_ativo (
    codigo,
    codigo_necessidade_manutencao,
	codigo_empresa,
	codigo_ativo,
	descricao,
	observacao
  ) VALUES (
    p_codigo,
    p_codigo_necessidade_manutencao,
	p_codigo_empresa,
	p_codigo_ativo,
	p_descricao,
	p_observacao
  )
  ON CONFLICT (codigo, codigo_necessidade_manutencao, codigo_empresa)
  DO UPDATE SET
    codigo_ativo   = EXCLUDED.codigo_ativo,
    descricao      = EXCLUDED.descricao,
    observacao     = EXCLUDED.observacao;

END;
$$;


ALTER PROCEDURE public.sp_necessidade_manutencao_upsert_ativo(IN p_codigo bigint, IN p_codigo_necessidade_manutencao bigint, IN p_codigo_empresa integer, IN p_codigo_ativo bigint, IN p_descricao text, IN p_observacao text) OWNER TO postgres;

--
-- Name: sp_necessidade_manutencao_upsert_ativo_item(bigint, bigint, bigint, integer, bigint, numeric, numeric, character); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_necessidade_manutencao_upsert_ativo_item(IN p_codigo bigint, IN p_codigo_ativo_vinculado bigint, IN p_codigo_necessidade_manutencao bigint, IN p_codigo_empresa integer, IN p_codigo_item_estoque bigint, IN p_quantidade numeric, IN p_valor_unitario numeric, IN p_tipo character)
    LANGUAGE plpgsql
    AS $$
BEGIN

  -- Faz o UPSERT: insere um novo item ou atualiza o existente com base no conflito
  INSERT INTO tb_manutencao_necessidade_ativo_item (
    codigo,
    codigo_ativo_vinculado,
	codigo_necessidade_manutencao,
	codigo_empresa,
	codigo_item_estoque,
	quantidade,
	valor_unitario,
	tipo
  ) VALUES (
    p_codigo,
    p_codigo_ativo_vinculado,
	p_codigo_necessidade_manutencao,
	p_codigo_empresa,
	p_codigo_item_estoque,
	p_quantidade,
	p_valor_unitario,
	p_tipo
  )
  ON CONFLICT (codigo, codigo_ativo_vinculado, codigo_necessidade_manutencao, codigo_empresa)
  DO UPDATE SET
    codigo_item_estoque   	= EXCLUDED.codigo_item_estoque,
    quantidade      		= EXCLUDED.quantidade,
    valor_unitario     		= EXCLUDED.valor_unitario,
	tipo 					= EXCLUDED.tipo;

END;
$$;


ALTER PROCEDURE public.sp_necessidade_manutencao_upsert_ativo_item(IN p_codigo bigint, IN p_codigo_ativo_vinculado bigint, IN p_codigo_necessidade_manutencao bigint, IN p_codigo_empresa integer, IN p_codigo_item_estoque bigint, IN p_quantidade numeric, IN p_valor_unitario numeric, IN p_tipo character) OWNER TO postgres;

--
-- Name: sp_ordem_servico_insert_os(integer, integer, bigint, text, date, date, bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_ordem_servico_insert_os(p_codigo_empresa integer, p_codigo_parceiro_negocio integer, p_codigo_ativo bigint, p_observacao text, p_data_criacao date, p_data_ultima_alteracao date, p_codigo_usuario_ultima_alteracao bigint) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_codigo integer;
BEGIN
    -- Gera o próximo código da OS
    SELECT COALESCE(MAX(codigo), 0) + 1 INTO v_codigo 
    FROM tb_manutencao_ordem_servico 
    WHERE codigo_empresa = p_codigo_empresa;

    -- Insere a nova OS na tabela
    INSERT INTO tb_manutencao_ordem_servico (			 			
        codigo_empresa, 		      			
        codigo_parceiro_negocio,				
        codigo_ativo,			  			
        observacao, 			      			
        data_criacao,						
        data_ultima_alteracao,				
        codigo_usuario_ultima_alteracao,
        codigo
    ) VALUES (
        p_codigo_empresa,
        p_codigo_parceiro_negocio,
        p_codigo_ativo,
        p_observacao,
        p_data_criacao,
        p_data_ultima_alteracao,
        p_codigo_usuario_ultima_alteracao,  
        v_codigo
    );
    
    -- Retorna o código da ordem de serviço criada
    RETURN v_codigo;
END;
$$;


ALTER FUNCTION public.sp_ordem_servico_insert_os(p_codigo_empresa integer, p_codigo_parceiro_negocio integer, p_codigo_ativo bigint, p_observacao text, p_data_criacao date, p_data_ultima_alteracao date, p_codigo_usuario_ultima_alteracao bigint) OWNER TO postgres;

--
-- Name: sp_ordem_servico_insert_os_item_criacao(integer, bigint, bigint, numeric, numeric, date, bigint); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_ordem_servico_insert_os_item_criacao(IN p_codigo_empresa integer, IN p_codigo_os bigint, IN p_codigo_item bigint, IN p_valor numeric, IN p_quantidade numeric, IN p_data_ultima_alteracao date, IN p_codigo_usuario_ultima_alteracao bigint)
    LANGUAGE plpgsql
    AS $$
	DECLARE
    v_codigo integer;
BEGIN
    -- Gera o próximo código item
    SELECT COALESCE(MAX(codigo), 0) + 1 INTO v_codigo 
    FROM tb_manutencao_ordem_servico_item 
    WHERE codigo_empresa = p_codigo_empresa
	AND   codigo_ordem_servico = p_codigo_os;

    INSERT INTO tb_manutencao_ordem_servico_item (			 			
    	codigo ,
    	codigo_empresa ,
    	codigo_ordem_servico,
    	codigo_item ,
    	quantidade ,
    	valor_unitario ,
		data_ultima_alteracao ,
	    codigo_usuario_ultima_alteracao 
    ) VALUES (
		v_codigo,
   		p_codigo_empresa ,
   		p_codigo_os ,
   		p_codigo_item ,
   		p_quantidade ,
		p_valor,
   		p_data_ultima_alteracao ,
   		p_codigo_usuario_ultima_alteracao 
	
    );
    
END;
$$;


ALTER PROCEDURE public.sp_ordem_servico_insert_os_item_criacao(IN p_codigo_empresa integer, IN p_codigo_os bigint, IN p_codigo_item bigint, IN p_valor numeric, IN p_quantidade numeric, IN p_data_ultima_alteracao date, IN p_codigo_usuario_ultima_alteracao bigint) OWNER TO postgres;

--
-- Name: sp_ordem_servico_update_dados(bigint, integer, integer, integer, character varying, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_ordem_servico_update_dados(IN p_codigo_ordem_servico bigint, IN p_codigo_empresa integer, IN p_codigo_cliente integer, IN p_codigo_ativo integer, IN p_observacao character varying, IN p_codigo_usuario_ultima_alteracao integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE tb_manutencao_ordem_servico
    SET codigo_parceiro_negocio 			= p_codigo_cliente,
        codigo_ativo		 				= p_codigo_ativo,
        observacao 							= p_observacao,
        data_ultima_alteracao 				= NOW(),
        codigo_usuario_ultima_alteracao 	= p_codigo_usuario_ultima_alteracao
    WHERE 	codigo = p_codigo_ordem_servico 
    AND 	codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER PROCEDURE public.sp_ordem_servico_update_dados(IN p_codigo_ordem_servico bigint, IN p_codigo_empresa integer, IN p_codigo_cliente integer, IN p_codigo_ativo integer, IN p_observacao character varying, IN p_codigo_usuario_ultima_alteracao integer) OWNER TO postgres;

--
-- Name: sp_ordem_servico_update_item(bigint, integer, integer, integer, double precision, double precision, bigint); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_ordem_servico_update_item(IN p_codigo_item bigint, IN p_codigo_empresa integer, IN p_codigo_os integer, IN p_codigo_item_estoque integer, IN p_quantidade double precision, IN p_valor_unitario double precision, IN p_codigo_usuario_ultima_alteracao bigint)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE tb_manutencao_ordem_servico_item
    SET 
        codigo_item 						= p_codigo_item_estoque,
		codigo_ordem_servico				= p_codigo_os,
		quantidade							= p_quantidade,
		valor_unitario						= p_valor_unitario,
        data_ultima_alteracao 				= NOW(),
        codigo_usuario_ultima_alteracao 	= p_codigo_usuario_ultima_alteracao
    WHERE 	codigo = p_codigo_item 
    AND 	codigo_empresa = p_codigo_empresa
	AND 	codigo_ordem_servico = p_codigo_os;
END;
$$;


ALTER PROCEDURE public.sp_ordem_servico_update_item(IN p_codigo_item bigint, IN p_codigo_empresa integer, IN p_codigo_os integer, IN p_codigo_item_estoque integer, IN p_quantidade double precision, IN p_valor_unitario double precision, IN p_codigo_usuario_ultima_alteracao bigint) OWNER TO postgres;

--
-- Name: sp_update_cadastro_basico_ambiente(integer, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_update_cadastro_basico_ambiente(IN ambiente_id integer, IN novo_nome_ambiente character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE tb_ambientes
    SET nome_ambiente = novo_nome_ambiente
    WHERE id = ambiente_id;
END;
$$;


ALTER PROCEDURE public.sp_update_cadastro_basico_ambiente(IN ambiente_id integer, IN novo_nome_ambiente character varying) OWNER TO postgres;

--
-- Name: sp_update_cadastro_basico_ativo(integer, integer, character varying, integer, character varying, character varying, integer, text, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_update_cadastro_basico_ativo(IN p_codigo integer, IN p_codigo_cliente integer, IN p_numero_serie character varying, IN p_codigo_fabricante integer, IN p_modelo character varying, IN p_observacao character varying, IN p_codigo_empresa integer, IN p_descricao text, IN p_alias character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE tb_cad_ativo
    SET
        codigo_cliente = p_codigo_cliente,
		numero_serie = p_numero_serie,
		codigo_fabricante = p_codigo_fabricante,
		modelo = p_modelo,
		observacao = p_observacao,
		descricao = p_descricao,
		alias = p_alias,
		data_ultima_alteracao = CURRENT_DATE
    WHERE codigo = p_codigo
	AND   codigo_empresa = p_codigo_empresa;
END;
$$;


ALTER PROCEDURE public.sp_update_cadastro_basico_ativo(IN p_codigo integer, IN p_codigo_cliente integer, IN p_numero_serie character varying, IN p_codigo_fabricante integer, IN p_modelo character varying, IN p_observacao character varying, IN p_codigo_empresa integer, IN p_descricao text, IN p_alias character varying) OWNER TO postgres;

--
-- Name: sp_update_cadastro_basico_ativo_v2(integer, integer, character varying, integer, character varying, smallint, integer, character varying, boolean, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_update_cadastro_basico_ativo_v2(IN p_codigo integer, IN p_codigo_cliente integer, IN p_numero_serie character varying, IN p_codigo_fabricante integer, IN p_modelo character varying, IN p_codigo_prioridade smallint, IN p_codigo_tecnico_responsavel integer, IN p_observacao character varying, IN p_nivel_manutencao boolean, IN p_codigo_empresa integer)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE tb_cad_ativo
    SET
        codigo_cliente = p_codigo_cliente,
        numero_serie = p_numero_serie,
        codigo_fabricante = p_codigo_fabricante,
        modelo = p_modelo,
        codigo_prioridade = p_codigo_prioridade,
        codigo_tecnico_responsavel = p_codigo_tecnico_responsavel,
        observacao = p_observacao,
        nivel_manutencao = p_nivel_manutencao,
        codigo_empresa = p_codigo_empresa,
        data_input = NOW() -- Atualizando com a data e hora atual
    WHERE codigo = p_codigo;
END;
$$;


ALTER PROCEDURE public.sp_update_cadastro_basico_ativo_v2(IN p_codigo integer, IN p_codigo_cliente integer, IN p_numero_serie character varying, IN p_codigo_fabricante integer, IN p_modelo character varying, IN p_codigo_prioridade smallint, IN p_codigo_tecnico_responsavel integer, IN p_observacao character varying, IN p_nivel_manutencao boolean, IN p_codigo_empresa integer) OWNER TO postgres;

--
-- Name: sp_usuario_atualizar_senha(integer, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_usuario_atualizar_senha(IN user_id integer, IN nova_senha character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE tb_cad_usuario
    SET senha = nova_senha
    WHERE id = user_id;
END;
$$;


ALTER PROCEDURE public.sp_usuario_atualizar_senha(IN user_id integer, IN nova_senha character varying) OWNER TO postgres;

--
-- Name: sp_usuario_cadastro(character varying, character varying, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_usuario_cadastro(IN nome character varying, IN email character varying, IN senha character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO tb_cad_usuario (nome, email, senha)
    VALUES (nome, email, senha);
END;
$$;


ALTER PROCEDURE public.sp_usuario_cadastro(IN nome character varying, IN email character varying, IN senha character varying) OWNER TO postgres;

--
-- Name: sp_validar_login(character varying, character varying, bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.sp_validar_login(p_usuario character varying, p_senha character varying, p_codigo_empresa bigint) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
DECLARE
    resultado boolean;
BEGIN
    SELECT EXISTS (
        SELECT 1
        FROM public.tb_cad_usuario
        WHERE usuario = p_usuario AND senha = p_senha AND codigo_empresa = p_codigo_empresa
    ) INTO resultado;

    RETURN resultado;
END;
$$;


ALTER FUNCTION public.sp_validar_login(p_usuario character varying, p_senha character varying, p_codigo_empresa bigint) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: max_codigo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.max_codigo (
    "?column?" integer
);


ALTER TABLE public.max_codigo OWNER TO postgres;

--
-- Name: municipio; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.municipio (
    id integer NOT NULL,
    codigo integer NOT NULL,
    nome character varying(255) NOT NULL,
    uf character(2) NOT NULL
);


ALTER TABLE public.municipio OWNER TO postgres;

--
-- Name: municipio_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.municipio_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.municipio_id_seq OWNER TO postgres;

--
-- Name: municipio_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.municipio_id_seq OWNED BY public.municipio.id;


--
-- Name: pais; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.pais (
    id integer NOT NULL,
    nome character varying(255),
    nome_pt character varying(255),
    sigla character(2),
    bacen integer
);


ALTER TABLE public.pais OWNER TO postgres;

--
-- Name: pais_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.pais_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.pais_id_seq OWNER TO postgres;

--
-- Name: pais_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.pais_id_seq OWNED BY public.pais.id;


--
-- Name: tb_agendamento_ativo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_agendamento_ativo (
    codigo bigint NOT NULL,
    codigo_empresa integer NOT NULL,
    codigo_agendamento integer NOT NULL,
    codigo_ativo integer
);


ALTER TABLE public.tb_agendamento_ativo OWNER TO postgres;

--
-- Name: tb_ambientes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_ambientes (
    id integer NOT NULL,
    nome_ambiente character varying(255) NOT NULL
);


ALTER TABLE public.tb_ambientes OWNER TO postgres;

--
-- Name: tb_ambientes_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tb_ambientes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tb_ambientes_id_seq OWNER TO postgres;

--
-- Name: tb_ambientes_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tb_ambientes_id_seq OWNED BY public.tb_ambientes.id;


--
-- Name: tb_cad_ativo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_cad_ativo (
    codigo integer NOT NULL,
    codigo_cliente integer,
    numero_serie character varying(255),
    codigo_fabricante integer,
    modelo character varying,
    observacao character varying,
    data_input date DEFAULT CURRENT_DATE,
    codigo_empresa integer NOT NULL,
    descricao text,
    alias character varying(255),
    data_ultima_alteracao date
);


ALTER TABLE public.tb_cad_ativo OWNER TO postgres;

--
-- Name: tb_cad_ativo_foto; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_cad_ativo_foto (
    codigo integer NOT NULL,
    codigo_empresa integer NOT NULL,
    titulo character varying(255) NOT NULL,
    caminho_completo character varying(500) NOT NULL,
    data_upload timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    descricao character varying(255),
    codigo_ativo integer NOT NULL
);


ALTER TABLE public.tb_cad_ativo_foto OWNER TO postgres;

--
-- Name: tb_cad_fabricante; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_cad_fabricante (
    codigo bigint NOT NULL,
    codigo_empresa smallint NOT NULL,
    nome_fantasia character varying,
    razao_social character varying,
    descricao text,
    ativo boolean
);


ALTER TABLE public.tb_cad_fabricante OWNER TO postgres;

--
-- Name: tb_cad_item; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_cad_item (
    codigo integer NOT NULL,
    nome_item character varying(255) NOT NULL,
    preco_base_venda numeric(10,2) NOT NULL,
    custo numeric(10,2) NOT NULL,
    codigo_empresa integer NOT NULL,
    data_input timestamp without time zone DEFAULT now()
);


ALTER TABLE public.tb_cad_item OWNER TO postgres;

--
-- Name: tb_cad_item_codigo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tb_cad_item_codigo_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tb_cad_item_codigo_seq OWNER TO postgres;

--
-- Name: tb_cad_item_codigo_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tb_cad_item_codigo_seq OWNED BY public.tb_cad_item.codigo;


--
-- Name: tb_cad_parceiro_negocio; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_cad_parceiro_negocio (
    codigo integer NOT NULL,
    is_cnpj boolean NOT NULL,
    documento character varying(18) NOT NULL,
    cep character varying(10),
    telefone character varying(20),
    email character varying(255),
    data_cadastro timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    tipo_parceiro character(1) NOT NULL,
    codigo_empresa integer NOT NULL,
    nome_fantasia character varying,
    razao_social character varying,
    logradouro character varying,
    numero character varying,
    complemento character varying,
    bairro character varying,
    contato character varying,
    codigo_pais bigint,
    codigo_cidade bigint,
    codigo_estado integer,
    CONSTRAINT tb_cad_parceiro_negocio_tipo_parceiro_check CHECK ((tipo_parceiro = ANY (ARRAY['C'::bpchar, 'F'::bpchar, 'A'::bpchar])))
);


ALTER TABLE public.tb_cad_parceiro_negocio OWNER TO postgres;

--
-- Name: tb_cad_parceiro_negocio_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tb_cad_parceiro_negocio_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tb_cad_parceiro_negocio_id_seq OWNER TO postgres;

--
-- Name: tb_cad_parceiro_negocio_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tb_cad_parceiro_negocio_id_seq OWNED BY public.tb_cad_parceiro_negocio.codigo;


--
-- Name: tb_cad_tecnico; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_cad_tecnico (
    codigo integer NOT NULL,
    nome character varying(255) NOT NULL,
    codigo_empresa integer NOT NULL,
    ativo boolean,
    data_input date
);


ALTER TABLE public.tb_cad_tecnico OWNER TO postgres;

--
-- Name: tb_cad_tecnico_codigo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tb_cad_tecnico_codigo_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tb_cad_tecnico_codigo_seq OWNER TO postgres;

--
-- Name: tb_cad_tecnico_codigo_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tb_cad_tecnico_codigo_seq OWNED BY public.tb_cad_tecnico.codigo;


--
-- Name: tb_cad_usuario; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_cad_usuario (
    codigo integer NOT NULL,
    usuario character varying(255) NOT NULL,
    email character varying(255) NOT NULL,
    senha character varying(255) NOT NULL,
    data_input timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    codigo_empresa bigint
);


ALTER TABLE public.tb_cad_usuario OWNER TO postgres;

--
-- Name: tb_cad_usuario_codigo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tb_cad_usuario_codigo_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tb_cad_usuario_codigo_seq OWNER TO postgres;

--
-- Name: tb_cad_usuario_codigo_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tb_cad_usuario_codigo_seq OWNED BY public.tb_cad_usuario.codigo;


--
-- Name: tb_cfg_caminho_arquivos_gerais; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_cfg_caminho_arquivos_gerais (
    imagem_ativo character varying
);


ALTER TABLE public.tb_cfg_caminho_arquivos_gerais OWNER TO postgres;

--
-- Name: tb_info_empresa; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_info_empresa (
    codigo integer DEFAULT nextval('public.tb_cad_usuario_codigo_seq'::regclass) NOT NULL,
    razao_social character varying(255) NOT NULL,
    nome_fantasia character varying(255) NOT NULL,
    documento character varying(255) NOT NULL
);


ALTER TABLE public.tb_info_empresa OWNER TO postgres;

--
-- Name: tb_manutencao_agendamento; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_agendamento (
    codigo integer NOT NULL,
    codigo_empresa integer NOT NULL,
    data_input date,
    data_agendada timestamp without time zone,
    codigo_tipo_manutencao integer,
    codigo_parceiro_negocio integer,
    descricao character varying,
    observacao character varying
);


ALTER TABLE public.tb_manutencao_agendamento OWNER TO postgres;

--
-- Name: tb_manutencao_necessidade; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_necessidade (
    codigo bigint NOT NULL,
    codigo_empresa integer NOT NULL,
    solicitante character varying,
    aprovador character varying,
    descricao text,
    observacao text,
    codigo_parceiro_negocio integer,
    nome_contato character varying,
    metodo_contato character varying,
    data_input date,
    data_ultima_alteracao date,
    codigo_usuario_ultima_alteracao integer,
    codigo_stc_tipo_manutencao integer,
    codigo_stc_nivel_prioridade integer,
    desconto_bruto_geral numeric,
    acrescimo_bruto_geral numeric,
    codigo_stc_status_nm integer
);


ALTER TABLE public.tb_manutencao_necessidade OWNER TO postgres;

--
-- Name: tb_manutencao_necessidade_ativo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_necessidade_ativo (
    codigo bigint NOT NULL,
    codigo_necessidade_manutencao bigint NOT NULL,
    codigo_empresa integer NOT NULL,
    codigo_ativo bigint,
    descricao text,
    observacao text
);


ALTER TABLE public.tb_manutencao_necessidade_ativo OWNER TO postgres;

--
-- Name: tb_manutencao_necessidade_ativo_imagem; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_necessidade_ativo_imagem (
    codigo bigint NOT NULL,
    codigo_ativo_vinculado bigint NOT NULL,
    codigo_necessidade_manutencao bigint NOT NULL,
    codigo_empresa integer NOT NULL,
    titulo character varying,
    caminho_completo character varying,
    data_input date
);


ALTER TABLE public.tb_manutencao_necessidade_ativo_imagem OWNER TO postgres;

--
-- Name: tb_manutencao_necessidade_ativo_item; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_necessidade_ativo_item (
    codigo bigint NOT NULL,
    codigo_ativo_vinculado bigint NOT NULL,
    codigo_necessidade_manutencao bigint NOT NULL,
    codigo_empresa integer NOT NULL,
    codigo_item_estoque bigint,
    quantidade numeric,
    valor_unitario numeric,
    tipo character(1)
);


ALTER TABLE public.tb_manutencao_necessidade_ativo_item OWNER TO postgres;

--
-- Name: tb_manutencao_necessidade_checklist; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_necessidade_checklist (
    codigo bigint NOT NULL,
    codigo_nm bigint NOT NULL,
    codigo_empresa integer NOT NULL,
    is_completa boolean,
    titulo_checklist character varying,
    observacao_checklist text
);


ALTER TABLE public.tb_manutencao_necessidade_checklist OWNER TO postgres;

--
-- Name: tb_manutencao_necessidade_checklist_item; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_necessidade_checklist_item (
    codigo integer NOT NULL,
    codigo_checklist bigint NOT NULL,
    codigo_nm bigint,
    codigo_empresa integer NOT NULL,
    titulo character varying,
    descricao text,
    is_atendido boolean
);


ALTER TABLE public.tb_manutencao_necessidade_checklist_item OWNER TO postgres;

--
-- Name: tb_manutencao_ordem_servico; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_ordem_servico (
    codigo integer NOT NULL,
    codigo_empresa integer NOT NULL,
    codigo_parceiro_negocio integer,
    observacao character varying,
    data_criacao date,
    data_ultima_alteracao date,
    codigo_usuario_ultima_alteracao bigint,
    codigo_tecnico integer,
    status integer,
    codigo_tipo_manutencao integer
);


ALTER TABLE public.tb_manutencao_ordem_servico OWNER TO postgres;

--
-- Name: tb_manutencao_ordem_servico_ativo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_ordem_servico_ativo (
    codigo integer NOT NULL,
    codigo_ativo integer,
    codigo_empresa integer NOT NULL,
    codigo_os bigint NOT NULL
);


ALTER TABLE public.tb_manutencao_ordem_servico_ativo OWNER TO postgres;

--
-- Name: tb_manutencao_ordem_servico_item; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_ordem_servico_item (
    codigo integer NOT NULL,
    codigo_empresa integer NOT NULL,
    codigo_ordem_servico integer NOT NULL,
    codigo_item integer,
    quantidade double precision,
    valor_unitario double precision,
    data_ultima_alteracao date,
    codigo_usuario_ultima_alteracao bigint,
    codigo_ativo_vinculado integer
);


ALTER TABLE public.tb_manutencao_ordem_servico_item OWNER TO postgres;

--
-- Name: tb_manutencao_ordem_servico_tecnico; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_manutencao_ordem_servico_tecnico (
    codigo_os bigint NOT NULL,
    codigo_tecnico integer NOT NULL,
    codigo_empresa integer NOT NULL
);


ALTER TABLE public.tb_manutencao_ordem_servico_tecnico OWNER TO postgres;

--
-- Name: tb_stc_estados; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_stc_estados (
    id integer NOT NULL,
    nome character varying(75) DEFAULT NULL::character varying,
    uf character varying(2) DEFAULT NULL::character varying,
    ibge integer,
    pais integer,
    ddd character varying(50) DEFAULT NULL::character varying
);


ALTER TABLE public.tb_stc_estados OWNER TO postgres;

--
-- Name: tb_stc_nivel_prioridade; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_stc_nivel_prioridade (
    codigo integer NOT NULL,
    nivel character varying(255) NOT NULL
);


ALTER TABLE public.tb_stc_nivel_prioridade OWNER TO postgres;

--
-- Name: tb_stc_nivel_prioridade_codigo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tb_stc_nivel_prioridade_codigo_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tb_stc_nivel_prioridade_codigo_seq OWNER TO postgres;

--
-- Name: tb_stc_nivel_prioridade_codigo_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tb_stc_nivel_prioridade_codigo_seq OWNED BY public.tb_stc_nivel_prioridade.codigo;


--
-- Name: tb_stc_status_nm; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_stc_status_nm (
    codigo integer NOT NULL,
    descricao character varying
);


ALTER TABLE public.tb_stc_status_nm OWNER TO postgres;

--
-- Name: tb_stc_tipo_manutencao; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tb_stc_tipo_manutencao (
    codigo integer NOT NULL,
    descricao character varying,
    ativo boolean
);


ALTER TABLE public.tb_stc_tipo_manutencao OWNER TO postgres;

--
-- Name: municipio id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.municipio ALTER COLUMN id SET DEFAULT nextval('public.municipio_id_seq'::regclass);


--
-- Name: pais id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pais ALTER COLUMN id SET DEFAULT nextval('public.pais_id_seq'::regclass);


--
-- Name: tb_ambientes id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_ambientes ALTER COLUMN id SET DEFAULT nextval('public.tb_ambientes_id_seq'::regclass);


--
-- Name: tb_cad_item codigo; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_item ALTER COLUMN codigo SET DEFAULT nextval('public.tb_cad_item_codigo_seq'::regclass);


--
-- Name: tb_cad_parceiro_negocio codigo; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_parceiro_negocio ALTER COLUMN codigo SET DEFAULT nextval('public.tb_cad_parceiro_negocio_id_seq'::regclass);


--
-- Name: tb_cad_tecnico codigo; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_tecnico ALTER COLUMN codigo SET DEFAULT nextval('public.tb_cad_tecnico_codigo_seq'::regclass);


--
-- Name: tb_cad_usuario codigo; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_usuario ALTER COLUMN codigo SET DEFAULT nextval('public.tb_cad_usuario_codigo_seq'::regclass);


--
-- Name: tb_stc_nivel_prioridade codigo; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_stc_nivel_prioridade ALTER COLUMN codigo SET DEFAULT nextval('public.tb_stc_nivel_prioridade_codigo_seq'::regclass);


--
-- Name: municipio municipio_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.municipio
    ADD CONSTRAINT municipio_pkey PRIMARY KEY (id);


--
-- Name: pais pais_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.pais
    ADD CONSTRAINT pais_pkey PRIMARY KEY (id);


--
-- Name: tb_stc_status_nm pk_codigo_status_manutencao; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_stc_status_nm
    ADD CONSTRAINT pk_codigo_status_manutencao PRIMARY KEY (codigo);


--
-- Name: tb_cad_ativo_foto pk_tb_cad_ativo_foto; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_ativo_foto
    ADD CONSTRAINT pk_tb_cad_ativo_foto PRIMARY KEY (codigo, codigo_empresa, codigo_ativo);


--
-- Name: tb_info_empresa pk_tb_info_empresa; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_info_empresa
    ADD CONSTRAINT pk_tb_info_empresa PRIMARY KEY (codigo);


--
-- Name: tb_stc_tipo_manutencao pk_tipo_manutencao; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_stc_tipo_manutencao
    ADD CONSTRAINT pk_tipo_manutencao PRIMARY KEY (codigo);


--
-- Name: tb_agendamento_ativo tb_agendamento_ativo_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_agendamento_ativo
    ADD CONSTRAINT tb_agendamento_ativo_pkey PRIMARY KEY (codigo, codigo_empresa, codigo_agendamento);


--
-- Name: tb_ambientes tb_ambientes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_ambientes
    ADD CONSTRAINT tb_ambientes_pkey PRIMARY KEY (id);


--
-- Name: tb_cad_ativo tb_cad_ativo_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_ativo
    ADD CONSTRAINT tb_cad_ativo_pkey PRIMARY KEY (codigo, codigo_empresa);


--
-- Name: tb_cad_fabricante tb_cad_fabricante_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_fabricante
    ADD CONSTRAINT tb_cad_fabricante_pkey PRIMARY KEY (codigo, codigo_empresa);


--
-- Name: tb_cad_item tb_cad_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_item
    ADD CONSTRAINT tb_cad_item_pkey PRIMARY KEY (codigo, codigo_empresa);


--
-- Name: tb_cad_parceiro_negocio tb_cad_parceiro_negocio_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_parceiro_negocio
    ADD CONSTRAINT tb_cad_parceiro_negocio_pkey PRIMARY KEY (codigo, codigo_empresa);


--
-- Name: tb_cad_tecnico tb_cad_tecnico_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_tecnico
    ADD CONSTRAINT tb_cad_tecnico_pkey PRIMARY KEY (codigo, codigo_empresa);


--
-- Name: tb_cad_usuario tb_cad_usuario_email_empresa_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_usuario
    ADD CONSTRAINT tb_cad_usuario_email_empresa_key UNIQUE (email, codigo_empresa);


--
-- Name: tb_cad_usuario tb_cad_usuario_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_usuario
    ADD CONSTRAINT tb_cad_usuario_pkey PRIMARY KEY (codigo);


--
-- Name: tb_manutencao_agendamento tb_manutencao_agendamento_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_agendamento
    ADD CONSTRAINT tb_manutencao_agendamento_pkey PRIMARY KEY (codigo, codigo_empresa);


--
-- Name: tb_manutencao_necessidade_ativo_imagem tb_manutencao_necessidade_ativo_imagem_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_ativo_imagem
    ADD CONSTRAINT tb_manutencao_necessidade_ativo_imagem_pkey PRIMARY KEY (codigo, codigo_ativo_vinculado, codigo_necessidade_manutencao, codigo_empresa);


--
-- Name: tb_manutencao_necessidade_ativo_item tb_manutencao_necessidade_ativo_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_ativo_item
    ADD CONSTRAINT tb_manutencao_necessidade_ativo_item_pkey PRIMARY KEY (codigo, codigo_ativo_vinculado, codigo_necessidade_manutencao, codigo_empresa);


--
-- Name: tb_manutencao_necessidade_ativo tb_manutencao_necessidade_ativo_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_ativo
    ADD CONSTRAINT tb_manutencao_necessidade_ativo_pkey PRIMARY KEY (codigo, codigo_necessidade_manutencao, codigo_empresa);


--
-- Name: tb_manutencao_necessidade_checklist_item tb_manutencao_necessidade_checklist_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_checklist_item
    ADD CONSTRAINT tb_manutencao_necessidade_checklist_item_pkey PRIMARY KEY (codigo, codigo_checklist, codigo_empresa);


--
-- Name: tb_manutencao_necessidade_checklist tb_manutencao_necessidade_checklist_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_checklist
    ADD CONSTRAINT tb_manutencao_necessidade_checklist_pkey PRIMARY KEY (codigo, codigo_nm, codigo_empresa);


--
-- Name: tb_manutencao_necessidade tb_manutencao_necessidade_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade
    ADD CONSTRAINT tb_manutencao_necessidade_pkey PRIMARY KEY (codigo, codigo_empresa);


--
-- Name: tb_manutencao_ordem_servico_ativo tb_manutencao_ordem_servico_ativo_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_ordem_servico_ativo
    ADD CONSTRAINT tb_manutencao_ordem_servico_ativo_pkey PRIMARY KEY (codigo, codigo_empresa, codigo_os);


--
-- Name: tb_manutencao_ordem_servico_item tb_manutencao_ordem_servico_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_ordem_servico_item
    ADD CONSTRAINT tb_manutencao_ordem_servico_item_pkey PRIMARY KEY (codigo, codigo_empresa, codigo_ordem_servico);


--
-- Name: tb_manutencao_ordem_servico tb_manutencao_ordem_servico_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_ordem_servico
    ADD CONSTRAINT tb_manutencao_ordem_servico_pkey PRIMARY KEY (codigo, codigo_empresa);


--
-- Name: tb_manutencao_ordem_servico_tecnico tb_manutencao_ordem_servico_tecnico_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_ordem_servico_tecnico
    ADD CONSTRAINT tb_manutencao_ordem_servico_tecnico_pkey PRIMARY KEY (codigo_os, codigo_tecnico, codigo_empresa);


--
-- Name: tb_stc_nivel_prioridade tb_stc_nivel_prioridade_nivel_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_stc_nivel_prioridade
    ADD CONSTRAINT tb_stc_nivel_prioridade_nivel_key UNIQUE (nivel);


--
-- Name: tb_stc_nivel_prioridade tb_stc_nivel_prioridade_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_stc_nivel_prioridade
    ADD CONSTRAINT tb_stc_nivel_prioridade_pkey PRIMARY KEY (codigo);


--
-- Name: idx_documento; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX idx_documento ON public.tb_cad_parceiro_negocio USING btree (documento);


--
-- Name: tb_cad_ativo_foto fk_codigo_ativo; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_ativo_foto
    ADD CONSTRAINT fk_codigo_ativo FOREIGN KEY (codigo_ativo, codigo_empresa) REFERENCES public.tb_cad_ativo(codigo, codigo_empresa) ON DELETE CASCADE;


--
-- Name: tb_manutencao_ordem_servico_item fk_constraint_os_item; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_ordem_servico_item
    ADD CONSTRAINT fk_constraint_os_item FOREIGN KEY (codigo_empresa, codigo_ordem_servico) REFERENCES public.tb_manutencao_ordem_servico(codigo_empresa, codigo) ON DELETE CASCADE;


--
-- Name: tb_manutencao_necessidade_checklist fk_manutencao_checklist; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_checklist
    ADD CONSTRAINT fk_manutencao_checklist FOREIGN KEY (codigo_nm, codigo_empresa) REFERENCES public.tb_manutencao_necessidade(codigo, codigo_empresa);


--
-- Name: tb_manutencao_necessidade_checklist_item fk_manutencao_checklist_item; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_checklist_item
    ADD CONSTRAINT fk_manutencao_checklist_item FOREIGN KEY (codigo_checklist, codigo_nm, codigo_empresa) REFERENCES public.tb_manutencao_necessidade_checklist(codigo, codigo_nm, codigo_empresa);


--
-- Name: tb_agendamento_ativo tb_agendamento_ativo_codigo_agendamento_codigo_empresa_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_agendamento_ativo
    ADD CONSTRAINT tb_agendamento_ativo_codigo_agendamento_codigo_empresa_fkey FOREIGN KEY (codigo_agendamento, codigo_empresa) REFERENCES public.tb_manutencao_agendamento(codigo, codigo_empresa);


--
-- Name: tb_cad_item tb_cad_item_codigo_empresa_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_cad_item
    ADD CONSTRAINT tb_cad_item_codigo_empresa_fkey FOREIGN KEY (codigo_empresa) REFERENCES public.tb_info_empresa(codigo);


--
-- Name: tb_manutencao_necessidade_ativo_item tb_manutencao_necessidade_at_codigo_ativo_vinculado_codig_fkey1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_ativo_item
    ADD CONSTRAINT tb_manutencao_necessidade_at_codigo_ativo_vinculado_codig_fkey1 FOREIGN KEY (codigo_ativo_vinculado, codigo_necessidade_manutencao, codigo_empresa) REFERENCES public.tb_manutencao_necessidade_ativo(codigo, codigo_necessidade_manutencao, codigo_empresa);


--
-- Name: tb_manutencao_necessidade_ativo_imagem tb_manutencao_necessidade_ati_codigo_ativo_vinculado_codig_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_ativo_imagem
    ADD CONSTRAINT tb_manutencao_necessidade_ati_codigo_ativo_vinculado_codig_fkey FOREIGN KEY (codigo_ativo_vinculado, codigo_necessidade_manutencao, codigo_empresa) REFERENCES public.tb_manutencao_necessidade_ativo(codigo, codigo_necessidade_manutencao, codigo_empresa);


--
-- Name: tb_manutencao_necessidade_ativo tb_manutencao_necessidade_ati_codigo_necessidade_manutenca_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_necessidade_ativo
    ADD CONSTRAINT tb_manutencao_necessidade_ati_codigo_necessidade_manutenca_fkey FOREIGN KEY (codigo_necessidade_manutencao, codigo_empresa) REFERENCES public.tb_manutencao_necessidade(codigo, codigo_empresa);


--
-- Name: tb_manutencao_ordem_servico_ativo tb_manutencao_ordem_servico_ativo_codigo_os_codigo_empresa_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_ordem_servico_ativo
    ADD CONSTRAINT tb_manutencao_ordem_servico_ativo_codigo_os_codigo_empresa_fkey FOREIGN KEY (codigo_os, codigo_empresa) REFERENCES public.tb_manutencao_ordem_servico(codigo, codigo_empresa);


--
-- Name: tb_manutencao_ordem_servico_tecnico tb_manutencao_ordem_servico_tecni_codigo_os_codigo_empresa_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tb_manutencao_ordem_servico_tecnico
    ADD CONSTRAINT tb_manutencao_ordem_servico_tecni_codigo_os_codigo_empresa_fkey FOREIGN KEY (codigo_os, codigo_empresa) REFERENCES public.tb_manutencao_ordem_servico(codigo, codigo_empresa);


--
-- PostgreSQL database dump complete
--

