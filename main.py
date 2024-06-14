import numpy as np
import matplotlib.pyplot as plt

# Parameters for the input distributions
mean_input1 = 10
std_input1 = 2
mean_input2 = 5
std_input2 = 1
mean_noise = 0
std_noise = 1

# Number of Monte Carlo simulations
num_simulations = 10000

# Generate random samples for input1, input2, and noise
input1_samples = np.random.normal(mean_input1, std_input1, num_simulations)
input2_samples = np.random.normal(mean_input2, std_input2, num_simulations)
noise_samples = np.random.normal(mean_noise, std_noise, num_simulations)

# Calculate the outcome variable
outcome_samples = input1_samples * input2_samples + noise_samples

# Plot the histogram of the outcome variable
plt.hist(outcome_samples, bins=50, density=True, alpha=0.7, color='blue')
plt.title('Monte Carlo Simulation of Outcome Variable')
plt.xlabel('Outcome')
plt.ylabel('Probability Density')
plt.grid(True)
plt.show()

# Print basic statistics
print(f"Mean of outcome: {np.mean(outcome_samples):.2f}")
print(f"Standard deviation of outcome: {np.std(outcome_samples):.2f}")
print(f"95% confidence interval of outcome: {np.percentile(outcome_samples, 2.5):.2f} to {np.percentile(outcome_samples, 97.5):.2f}")
