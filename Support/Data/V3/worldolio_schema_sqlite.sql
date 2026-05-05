DROP TABLE IF EXISTS sra_schema_revision_audit;
CREATE TABLE sra_schema_revision_audit (
	sra_description varchar(255) not null,
	sra_timestamp varchar(20) default current_timestamp
);

/* sra data */
insert into sra_schema_revision_audit (sra_description) values ('Schema Init v3.0 - start SQLIte v' || sqlite_version());

/* drop tables in the correct order */
DROP TABLE IF EXISTS cty_city;
DROP TABLE IF EXISTS cnt_country;
DROP TABLE IF EXISTS dsi_driveside;

CREATE TABLE dsi_driveside (
    dsi_id INTEGER NOT NULL PRIMARY KEY,
    dsi_description varchar(255) not null
);

/* dsi data */
insert into dsi_driveside (dsi_id,dsi_description) values (0,'Unknown');
insert into dsi_driveside (dsi_id,dsi_description) values (1,'Left');
insert into dsi_driveside (dsi_id,dsi_description) values (2,'Right');

CREATE TABLE cnt_country (
    cnt_iso2name varchar(255) not null PRIMARY KEY,
    cnt_iso3name varchar(255) not null,
	cnt_displayname varchar(255) not null,
	cnt_peoplename varchar(255) not null,
    cnt_isonumber INTEGER NOT NULL,
	cnt_intdialcode varchar(255),
	cnt_intaccesscode varchar(255),
	cnt_areacodeprefix varchar(255),
    cnt_dsi_id INTEGER not null
           CONSTRAINT fk_cnt_dsi REFERENCES dsi_driveside(dsi_id)
);

/* start triggers to support 
    CREATE TABLE cnt_country (
	........
    cnt_dsi_id INTEGER not null
           CONSTRAINT fk_cnt_dsi REFERENCES dsi_driveside(dsi_id)
	}
*/

-- Foreign Key Preventing insert
DROP TRIGGER IF EXISTS fk_cnt_dsi_ins;
CREATE TRIGGER fk_cnt_dsi_ins
BEFORE INSERT ON [cnt_country]
FOR EACH ROW BEGIN
  SELECT RAISE(ROLLBACK, 'insert on table "[cnt_country]" violates foreign key constraint "fk_cnt_dsi"')
  WHERE NEW.cnt_dsi_id IS NOT NULL AND (SELECT dsi_id FROM [dsi_driveside] WHERE dsi_id = NEW.cnt_dsi_id) IS NULL;
END;

-- Foreign key preventing update
DROP TRIGGER IF EXISTS fk_cnt_dsi_upd;
CREATE TRIGGER fk_cnt_dsi_upd
BEFORE UPDATE ON [cnt_country]
FOR EACH ROW BEGIN
    SELECT RAISE(ROLLBACK, 'update on table "[cnt_country]" violates foreign key constraint "fk_cnt_dsi"')
    WHERE NEW.cnt_dsi_id IS NOT NULL AND (SELECT dsi_id FROM [dsi_driveside] WHERE dsi_id = NEW.cnt_dsi_id) IS NULL;
END;

DROP TRIGGER IF EXISTS fk_cnt_dsi_del;
-- Foreign key preventing delete
CREATE TRIGGER fk_cnt_dsi_del
BEFORE DELETE ON [dsi_driveside]
FOR EACH ROW BEGIN
    SELECT RAISE(ROLLBACK, 'delete on table "[dsi_driveside]" violates foreign key constraint "fk_cnt_dsi"')
    WHERE (SELECT cnt_dsi_id FROM [cnt_country] WHERE cnt_dsi_id = OLD.dsi_id) IS NOT NULL;
END;

/* end triggers to support fk_cnt_dsi */


CREATE TABLE cty_city (
    cty_id INTEGER NOT NULL PRIMARY KEY,
	cty_displayname varchar(255) not null,
	cty_cnt_iso2name varchar(255) not null
           CONSTRAINT fk_cty_cnt REFERENCES cnt_country(cnt_iso2name),
    cty_windowstzindex INTEGER NOT NULL,
	cty_areacode varchar(255),
    cty_latitude INTEGER NOT NULL,
    cty_longitude INTEGER NOT NULL,
	cty_iataairportcode varchar(255) not null,
	cty_icaoairportcode varchar(255) not null,
	cty_ianatz varchar(255) not null
);

/* start triggers to support 
    CREATE TABLE cty_city (
	........
	cty_cnt_iso2name varchar(255) not null,
           CONSTRAINT fk_cty_cnt REFERENCES cnt_country(cnt_iso2name)
	}
*/

-- Foreign Key Preventing insert
DROP TRIGGER IF EXISTS fk_cty_cnt_ins;
CREATE TRIGGER fk_cty_cnt_ins
BEFORE INSERT ON [cty_city]
FOR EACH ROW BEGIN
  SELECT RAISE(ROLLBACK, 'insert on table "[cty_city]" violates foreign key constraint "fk_cty_cnt"')
  WHERE NEW.cty_cnt_iso2name IS NOT NULL AND (SELECT cnt_iso2name FROM [cnt_country] WHERE cnt_iso2name = NEW.cty_cnt_iso2name) IS NULL;
END;

-- Foreign key preventing update
DROP TRIGGER IF EXISTS fk_cty_cnt_upd;
CREATE TRIGGER fk_cty_cnt_upd
BEFORE UPDATE ON [cty_city]
FOR EACH ROW BEGIN
    SELECT RAISE(ROLLBACK, 'update on table "[cty_city]" violates foreign key constraint "fk_cty_cnt"')
    WHERE NEW.cty_cnt_iso2name IS NOT NULL AND (SELECT cnt_iso2name FROM [cnt_country] WHERE cnt_iso2name = NEW.cty_cnt_iso2name) IS NULL;
END;

DROP TRIGGER IF EXISTS fk_cty_cnt_del;
-- Foreign key preventing delete
CREATE TRIGGER fk_cty_cnt_del
BEFORE DELETE ON [cnt_country]
FOR EACH ROW BEGIN
    SELECT RAISE(ROLLBACK, 'delete on table "[cnt_country]" violates foreign key constraint "fk_cty_cnt"')
    WHERE (SELECT cty_cnt_iso2name FROM [cty_city] WHERE cty_cnt_iso2name = OLD.cnt_iso2name) IS NOT NULL;
END;

/* end triggers to support fk_cnt_dsi */


/* sra data */
insert into sra_schema_revision_audit (sra_description) values ('Schema Init - end');


/* data */
insert into sra_schema_revision_audit (sra_description) values ('Schema Init Data Load v3.0 - start');

.mode csv
.import country.csv cnt_country
.import city.csv cty_city

insert into sra_schema_revision_audit (sra_description) values ('Schema Init Data Load v3.0 - end');

