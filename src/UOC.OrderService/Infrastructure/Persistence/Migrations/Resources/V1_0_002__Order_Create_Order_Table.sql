CREATE TABLE "order"."order"
(
	id uuid PRIMARY KEY,
	customer_id uuid NOT NULL,
    "status" varchar(50) NOT NULL,
    create_date timestamp NOT NULL,
    modification_date timestamp NOT NULL
);



