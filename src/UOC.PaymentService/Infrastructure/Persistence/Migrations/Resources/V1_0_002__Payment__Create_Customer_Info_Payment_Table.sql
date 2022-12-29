CREATE TABLE "payment"."customer_info_payment"
(
	customer_id uuid PRIMARY KEY,
	iban varchar(100) NOT NULL,
    create_date timestamp NOT NULL
);