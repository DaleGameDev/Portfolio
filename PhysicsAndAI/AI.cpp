#include "AI.h"



AI::AI()
{
	centerDirection = btVector3(0, 0, 1);
	centerPosition = btVector3(0, 30, 0);
	sleepTimer = 0;
	reflectTimer = 0;
	reflect = false;
	fleeTimer = 0;
	flee = false;
	fleeFrom = btVector3(0, 0, 0);
}


AI::~AI()
{
}


btVector3 AI::think(btVector3 position,bool &scared) {
	btVector3 newDirection;
	
	boidPositions.push_back(position);
	btVector3 boidToCentre =  centerPosition - position;
	//distance to centre of the school 
	btScalar length = boidToCentre.length();
	float tooFar, justRightL, justRightR, tooClose, justRight;
	//length used here in line equations to work out the Y for the fuzzy logic 
	tooClose = 6 - length;
	//The y is then clamped to between 0 and 1 this is repeated for each of the 3 areas of the fuzzy logic
	tooClose = std::max(0.0f, std::min(1.0f, tooClose));
	tooFar = -9 + length; 
	tooFar = std::max(0.0f, std::min(1.0f, tooFar));
	justRightL = -5 + length;
	justRightR = 9 - length;
	justRight = std::min(std::min(std::max(0.0f, justRightL), std::max(0.0f, justRightR)),1.0f);
	//the Y values are then added 
	float totalOdds = justRight + tooClose + tooFar;
	//each y is divided by the total to make the Y values add to 1
	justRight /= totalOdds; tooClose /= totalOdds; tooFar /= totalOdds;
	//random number between 0 and 1
	float rand = (float)std::rand() / RAND_MAX;

	//The Y is then defuzzied by each Y value being compared to the random number this adds an element of randomness to the 
	//what option is slected but it does not have a chance to select a completely wrong answer
	if (rand < tooClose) {
		//moves boid away from the centre of the school
		btVector3 dirToCentre = centerPosition - position;
		newDirection = -dirToCentre + 2 * centerDirection;
		newDirection.normalize();
	}
	else if (rand < justRight) {
		//moves the boid with the school with a random drift 
		btVector3 dirToCentre = centerPosition - position;
		newDirection = centerDirection+addRandom();
		newDirection.normalize();
	}
	else {
		//moves toward the centre of the school 
		btVector3 dirToCentre = centerPosition - position;
		newDirection = dirToCentre + centerDirection;
		newDirection.normalize();
	}
	
	//sorts to see what is the closest enemy 
	btVector3 closestEnemy(0,0,0);
	btScalar shortestDistance = 1000;
	for (unsigned int i = 0;i < enemyPositions.size();i++) {
		btVector3 boidToEnemy = enemyPositions[i] - position;
		btScalar distance = boidToEnemy.length();
		if (shortestDistance > distance) {
			shortestDistance = distance;
			closestEnemy = enemyPositions[i];
		}
	}

	//if the closest shortestDistance is less than 15 then the school goes into the flee state
	if (shortestDistance < 15) {
		flee = true;
		fleeTimer = 0;
		fleeFrom = closestEnemy;
		
	}
	//This means the boid has seen the enemy buit hasn't reacted yet to change the direction 
	if (shortestDistance < 18) {
		scared = true;
	}


	//return the direction of the boid 
	return newDirection;
}




//Decides the position and direction of the school
void AI::schoolThink()
{
	//checks if is in the flee state
	//this is fixed framerate dependent sets a count for how long the flee state is active 
	if (flee == true) {
		fleeTimer++;
		if (fleeTimer == 100) {
			fleeTimer = 0;
			flee = false;
		}
	}
	
	//Works out the position of the centre of the school 
	btVector3 sum(0,0,0);
	unsigned int i = 0;
	for (; i < boidPositions.size();i++) {
		sum += boidPositions[i];
	}
	centerPosition = sum / i;
	boidPositions.clear();
	//Adds to the sleep timer 
	sleepTimer++;
	//If the sleepTimer is up to 30 a small random factor is added to the direction 
	if (sleepTimer == 30) {
		addSmallRandom();
		sleepTimer = 0;
	}

	//Reflect timer so the boids can't reflect straight after being reflected to stop from getting stuck in a corner
	if (reflect == true) {
		reflectTimer++;
		if (reflectTimer == 20) {
			reflect = false;
			reflectTimer = 0;
			sleepTimer = 0;
		}
	}

	//if the school reaches the edge of the area it then heads back towards a random point in an imaginary smaller square...
	//in the centre of the area
	if (centerPosition.getX() > 150 && reflect==false) {
		int x = (rand() % 100) - 50;
		int y = (rand() % 50)+20;
		int z = (rand() % 100) - 50;
		centerDirection = btVector3(x, y, z) - centerPosition;
		//Adds another small random element to the direction
		addSmallRandom();
		reflect = true;
	}
	else if (centerPosition.getX() < -140 && reflect == false) {
		int x = (rand() % 100) - 50;
		int y = (rand() % 50) + 20;
		int z = (rand() % 100) - 50;
		centerDirection = btVector3(x, y, z) - centerPosition;
		addSmallRandom();
		reflect = true;
	}
	else if (centerPosition.getY() > 100 && reflect == false) {
		int x = (rand() % 100) - 50;
		int y = (rand() % 50) + 20;
		int z = (rand() % 100) - 50;
		centerDirection = btVector3(x, y, z) - centerPosition;
		addSmallRandom();
		reflect = true;
	}
	else if (centerPosition.getY() < 10 && reflect == false) {
		int x = (rand() % 100) - 50;
		int y = (rand() % 50) + 20;
		int z = (rand() % 100) - 50;
		centerDirection = btVector3(x, y, z) - centerPosition;
		addSmallRandom();
		reflect = true;
	}
	else if (centerPosition.getZ() > 140 && reflect == false) {
		int x = (rand() % 100) - 50;
		int y = (rand() % 50) + 20;
		int z = (rand() % 100) - 50;
		centerDirection = btVector3(x, y, z) - centerPosition;
		addSmallRandom();
		reflect = true;
	}
	else if (centerPosition.getZ() < -140 && reflect == false) {
		int x = (rand() % 100) - 50;
		int y = (rand() % 50) + 20;
		int z = (rand() % 100) - 50;
		centerDirection = btVector3(x, y, z) - centerPosition;
		addSmallRandom();
		reflect = true;
	}
	//responds to the flee state
	if (flee == true) {
		centerDirection = centerPosition - fleeFrom;
		addSmallRandom();
	}
	centerDirection.safeNormalize();

}

//random Vector 
btVector3 AI::addRandom() {
	float randX = (((float)rand() / RAND_MAX) * 2 - 1) /5;
	float randY = (((float)rand() / RAND_MAX) * 2 - 1) /5;
	float randZ = (((float)rand() / RAND_MAX) * 2 - 1) /5;

	return btVector3(randX, randY, randZ);
}

//random vector adding to the direction 
void AI::addSmallRandom() {
	float randX = (((float)rand() / RAND_MAX) * 2 - 1) / 10;
	float randY = (((float)rand() / RAND_MAX) * 2 - 1) / 10;
	float randZ = (((float)rand() / RAND_MAX) * 2 - 1) / 10;
	btScalar x = centerDirection.getX();
	btScalar y = centerDirection.getY();
	btScalar z = centerDirection.getZ();
	centerDirection.setX(x + randX);
	centerDirection.setY(y + randY);
	centerDirection.setZ(z + randZ);
}

//returns the flee state for use in deciding the speed of the boids, 
bool AI::getFlee()
{
	return flee;
}
