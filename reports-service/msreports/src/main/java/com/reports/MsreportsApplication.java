package com.reports;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling
public class MsreportsApplication {

	public static void main(String[] args) {
		SpringApplication.run(MsreportsApplication.class, args);
	}

}
