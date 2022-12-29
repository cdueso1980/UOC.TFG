CREATE TABLE "payment"."payment"
(
	id uuid PRIMARY KEY,
	customer_id uuid NOT NULL,
	order_id uuid NOT NULL,
    total_amount decimal NOT NULL,
    "status" varchar(50) NOT NULL,
    create_date timestamp NOT NULL,
    CONSTRAINT fk_payment_customer_info_payment_id FOREIGN KEY(customer_id) REFERENCES "payment"."customer_info_payment"(customer_id),
    UNIQUE (customer_id, order_id)
);